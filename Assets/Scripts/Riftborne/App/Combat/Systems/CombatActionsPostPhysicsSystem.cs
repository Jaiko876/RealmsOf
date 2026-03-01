using System;
using System.Collections.Generic;
using Riftborne.Core.Config;
using Riftborne.Core.Gameplay.Combat.Abstractions;
using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Gameplay.Combat.Rules.Abstractions;
using Riftborne.Core.Gameplay.Weapons.Abstractions;
using Riftborne.Core.Gameplay.Weapons.Model;
using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;
using Riftborne.Core.Stats;
using Riftborne.Core.Stores.Abstractions;
using Riftborne.Core.Systems.PostPhysicsTickSystems;

namespace Riftborne.App.Combat.Systems
{
    public sealed class CombatActionsPostPhysicsSystem : IPostPhysicsTickSystem
    {
        private readonly GameState _state;
        private readonly ICombatActionStore _actions;
        private readonly IBlockStateStore _block;

        private readonly ICombatHitQuery _hitQuery;
        private readonly IGameplayTuning _tuning;

        private readonly IWeaponCatalog _weapons;
        private readonly IEquippedWeaponStore _equipped;

        private readonly IStatsStore _stats;
        private readonly IStatsDeltaStore _deltas;
        private readonly IActionEventStore _events;

        private readonly ICombatRulesResolver _rules;

        private readonly GameEntityId[] _hitIds = new GameEntityId[32];
        private readonly HashSet<int> _dedupe = new HashSet<int>();

        private readonly List<GameEntityId> _toRemove = new List<GameEntityId>(32);

        public CombatActionsPostPhysicsSystem(
            GameState state,
            ICombatActionStore actions,
            ICombatHitQuery hitQuery,
            IGameplayTuning tuning,
            IWeaponCatalog weapons,
            IEquippedWeaponStore equipped,
            IStatsStore stats,
            IStatsDeltaStore deltas,
            ICombatRulesResolver rules, 
            IBlockStateStore block, 
            IActionEventStore events)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _hitQuery = hitQuery ?? throw new ArgumentNullException(nameof(hitQuery));
            _tuning = tuning ?? throw new ArgumentNullException(nameof(tuning));
            _weapons = weapons ?? throw new ArgumentNullException(nameof(weapons));
            _equipped = equipped ?? throw new ArgumentNullException(nameof(equipped));
            _stats = stats ?? throw new ArgumentNullException(nameof(stats));
            _deltas = deltas ?? throw new ArgumentNullException(nameof(deltas));
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _block = block ?? throw new ArgumentNullException(nameof(block));
            _events = events ?? throw new ArgumentNullException(nameof(events));
        }

        public void Tick(int tick)
        {
            _toRemove.Clear();

            foreach (var kv in _state.Entities)
            {
                var id = kv.Key;

                if (!_actions.TryGet(id, out var a))
                    continue;

                // end?
                if (!a.IsRunningAt(tick))
                {
                    _toRemove.Add(id);
                    continue;
                }

                // On first active tick of attacks: apply hit once
                if ((a.Type == CombatActionType.LightAttack || a.Type == CombatActionType.HeavyAttack)
                    && a.IsFirstActiveTick(tick)
                    && !a.HitApplied)
                {
                    ApplyAttackHits(tick, id, a);
                    _actions.Set(id, a.WithHitApplied(true));
                }
            }

            for (int i = 0; i < _toRemove.Count; i++)
                _actions.Remove(_toRemove[i]);
        }

        private void ApplyAttackHits(int tick, GameEntityId attackerId, CombatActionInstance attack)
        {
            if (!_state.Entities.TryGetValue(attackerId, out var attacker) || attacker == null)
                return;

            if (!_stats.TryGet(attackerId, out var attackerStats) || !attackerStats.IsInitialized)
                return;

            var weaponId = _equipped.GetOrDefault(attackerId, WeaponId.Fists);
            if (!_weapons.TryGet(weaponId, out var weaponDef))
                return;

            var hit = weaponDef.GetHit(attack.Type == CombatActionType.HeavyAttack
                ? ActionState.HeavyAttack
                : ActionState.LightAttack);

            int facing = attacker.Facing < 0 ? -1 : 1;

            float centerX = attacker.X + hit.OffsetX * facing;
            float centerY = attacker.Y + hit.OffsetY;

            int count = _hitQuery.OverlapBox(centerX, centerY, hit.Width, hit.Height, _tuning.CombatHit.TargetLayerMask, _hitIds);
            if (count <= 0)
                return;

            _dedupe.Clear();

            for (int i = 0; i < count; i++)
            {
                var defenderId = _hitIds[i];
                if (defenderId.Equals(attackerId))
                    continue;

                // dedupe by int id value
                if (!_dedupe.Add(defenderId.Value))
                    continue;

                if (!_state.Entities.ContainsKey(defenderId))
                    continue;

                ApplyHitToOne(tick, attackerId, defenderId, attack.Type);
            }
        }

        private void ApplyHitToOne(int tick, GameEntityId attackerId, GameEntityId defenderId, CombatActionType attackType)
        {
            if (!_stats.TryGet(attackerId, out var aStats) || !aStats.IsInitialized)
                return;

            if (!_stats.TryGet(defenderId, out var dStats) || !dStats.IsInitialized)
                return;

            bool parryActive = false;
            bool dodgeActive = false;
            
            CombatActionInstance defAction = default;

            if (_actions.TryGet(defenderId, out defAction) && defAction.IsRunningAt(tick))
            {
                parryActive = defAction.Type == CombatActionType.Parry && defAction.IsActiveAt(tick);

                // dodge считается активным и для DodgeDash тоже
                bool isDodgeType = defAction.Type == CombatActionType.Dodge || defAction.Type == CombatActionType.DodgeDash;
                dodgeActive = isDodgeType && defAction.IsActiveAt(tick);
            }

            bool blockActive = !dodgeActive && _block != null && _block.IsBlocking(defenderId);
            
            if (attackType == CombatActionType.HeavyAttack)
            {
                TryTriggerPerfectDodge(tick, defenderId, defAction, dodgeActive);
            }

            var req = new CombatResolveRequest(
                attackerId: attackerId,
                defenderId: defenderId,
                attack: attackType,
                defenderParryActive: parryActive,
                defenderDodgeActive: dodgeActive,
                defenderBlockActive: blockActive,
                attackerAttack: aStats.GetEffective(StatId.Attack),
                defenderDefense: dStats.GetEffective(StatId.Defense));

            var r = _rules.Resolve(in req);

            // Defender deltas
            if (r.DefenderHpDamage > 0) _deltas.DamageHp(defenderId, r.DefenderHpDamage, StatsDeltaKind.Damage);
            if (r.DefenderStaminaDamage > 0) _deltas.SpendStamina(defenderId, r.DefenderStaminaDamage, StatsDeltaKind.Cost);
            if (r.DefenderStaggerBuild > 0) _deltas.AddStagger(defenderId, r.DefenderStaggerBuild, StatsDeltaKind.Damage);

            // Attacker punish
            if (r.AttackerStaminaDamage > 0) _deltas.SpendStamina(attackerId, r.AttackerStaminaDamage, StatsDeltaKind.Cost);
            if (r.AttackerStaggerBuild > 0) _deltas.AddStagger(attackerId, r.AttackerStaggerBuild, StatsDeltaKind.Damage);
        }
        
        private void TryTriggerPerfectDodge(int tick, GameEntityId defenderId, CombatActionInstance cur, bool dodgeActive)
        {
            // нужен активный обычный dodge (не dash)
            if (!dodgeActive) return;
            if (cur.Type != CombatActionType.Dodge) return;

            // окно perfect из tuning (например 3 тика)
            int window = _tuning.CombatActions.PerfectDodge.WindowTicks;
            if (window <= 0) return;

            // elapsed в active: tick - (start + windup)
            int activeStart = cur.StartTick + cur.WindupTicks;
            int elapsedInActive = tick - activeStart;

            if (elapsedInActive < 0) return;
            if (elapsedInActive >= window) return; // не perfect

            // Стартуем dash прямо на этом же тике
            var dashCfg = _tuning.CombatActions.DodgeDash; // FixedAction
            int total = dashCfg.DurationBaseTicks;

            SplitPhases(total, dashCfg.Phases, out var w, out var a, out var r);

            // направление: если в action уже был залочен facing - берем его, иначе просто 1
            sbyte facing = cur.LockedFacing != 0 ? cur.LockedFacing : (sbyte)1;

            var dash = new CombatActionInstance(
                type: CombatActionType.DodgeDash,
                startTick: tick,
                windupTicks: w,
                activeTicks: a,
                recoveryTicks: r,
                cooldownTicks: 0,
                lockedFacing: facing,
                hitApplied: false);

            _actions.Set(defenderId, dash);

            // Пушим отдельное анимационное событие
            _events.SetTiming(defenderId, ActionState.DodgePerfect, total, tick);
            _events.SetIntent(defenderId, ActionState.DodgePerfect, tick);
        }
        
        private static void SplitPhases(int totalTicks, CombatActionsTuning.PhaseWeights w, out int windup, out int active, out int recovery)
        {
            if (totalTicks < 0) totalTicks = 0;

            int sum = w.WindupWeight + w.ActiveWeight + w.RecoveryWeight;
            if (sum <= 0 || totalTicks == 0)
            {
                windup = 0;
                active = 0;
                recovery = totalTicks;
                return;
            }

            windup = (int)MathF.Round(totalTicks * (w.WindupWeight / (float)sum));
            active = (int)MathF.Round(totalTicks * (w.ActiveWeight / (float)sum));

            if (windup < 0) windup = 0;
            if (active < 0) active = 0;

            int used = windup + active;
            if (used > totalTicks)
            {
                int overflow = used - totalTicks;
                if (active >= overflow) active -= overflow;
                else windup = Math.Max(0, windup - (overflow - active));
            }

            recovery = totalTicks - (windup + active);
            if (recovery < 0) recovery = 0;
        }
    }
}