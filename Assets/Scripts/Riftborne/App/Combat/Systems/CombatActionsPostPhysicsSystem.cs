// Assets/Scripts/Riftborne/App/Combat/Systems/CombatActionsPostPhysicsSystem.cs

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

        private readonly ICombatHitQuery _hitQuery;
        private readonly IGameplayTuning _tuning;

        private readonly IWeaponCatalog _weapons;
        private readonly IEquippedWeaponStore _equipped;

        private readonly IStatsStore _stats;
        private readonly IStatsDeltaStore _deltas;

        private readonly ICombatHitRules _rules;

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
            ICombatHitRules rules)
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

            if (_actions.TryGet(defenderId, out var defAction) && defAction.IsRunningAt(tick))
            {
                parryActive = defAction.Type == CombatActionType.Parry && defAction.IsActiveAt(tick);
                dodgeActive = defAction.Type == CombatActionType.Dodge && defAction.IsActiveAt(tick);
            }

            var ctx = new CombatHitContext(
                attack: attackType,
                defenderParryActive: parryActive,
                defenderDodgeActive: dodgeActive,
                attackerAttack: aStats.GetEffective(StatId.Attack),
                defenderDefense: dStats.GetEffective(StatId.Defense));

            var r = _rules.Resolve(in ctx);

            // Defender deltas
            if (r.DefenderHpDamage > 0) _deltas.DamageHp(defenderId, r.DefenderHpDamage, StatsDeltaKind.Damage);
            if (r.DefenderStaminaDamage > 0) _deltas.SpendStamina(defenderId, r.DefenderStaminaDamage, StatsDeltaKind.Cost);
            if (r.DefenderStaggerBuild > 0) _deltas.AddStagger(defenderId, r.DefenderStaggerBuild, StatsDeltaKind.Damage);

            // Attacker punish
            if (r.AttackerStaminaDamage > 0) _deltas.SpendStamina(attackerId, r.AttackerStaminaDamage, StatsDeltaKind.Cost);
            if (r.AttackerStaggerBuild > 0) _deltas.AddStagger(attackerId, r.AttackerStaggerBuild, StatsDeltaKind.Damage);
        }
    }
}