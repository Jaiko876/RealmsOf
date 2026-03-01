using System;
using Riftborne.App.Combat.Abstractions;
using Riftborne.Core.Config;
using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Gameplay.Combat.Rules.Abstractions;
using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;
using Riftborne.Core.Stats;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.App.Combat
{
    public sealed class CombatActionStarter : ICombatActionStarter
    {
        private readonly ICombatActionStore _actions;
        private readonly IActionEventStore _events;
        private readonly IAttackCooldownStore _attackCooldowns;
        private readonly ICombatActionCooldownStore _combatCooldowns;
        private readonly IGameplayTuning _tuning;
        private readonly ICombatCancelRules _cancelRules;

        private readonly IStatsStore _stats;
        private readonly IStatsDeltaStore _deltas;
        private readonly CombatResourceTuning _resources;

        public CombatActionStarter(
            ICombatActionStore actions,
            IActionEventStore events,
            IAttackCooldownStore attackCooldowns,
            ICombatActionCooldownStore combatCooldowns,
            IGameplayTuning tuning,
            ICombatCancelRules cancelRules,
            IStatsStore stats,
            IStatsDeltaStore deltas)
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _events = events ?? throw new ArgumentNullException(nameof(events));
            _attackCooldowns = attackCooldowns ?? throw new ArgumentNullException(nameof(attackCooldowns));
            _combatCooldowns = combatCooldowns ?? throw new ArgumentNullException(nameof(combatCooldowns));
            _tuning = tuning ?? throw new ArgumentNullException(nameof(tuning));
            _cancelRules = cancelRules ?? throw new ArgumentNullException(nameof(cancelRules));

            _stats = stats ?? throw new ArgumentNullException(nameof(stats));
            _deltas = deltas ?? throw new ArgumentNullException(nameof(deltas));
            _resources = _tuning.CombatResources;
        }

        public void TryStartAttack(GameEntityId id, int tick, ActionState action, int totalDurationTicks, int cooldownTicks, int facing)
        {
            var type = ToCombatActionType(action);
            if (type != CombatActionType.LightAttack && type != CombatActionType.HeavyAttack)
                return;

            if (IsBusy(id, tick))
                return;

            if (!_attackCooldowns.CanAttack(id, tick))
                return;

            int staminaCost = (type == CombatActionType.LightAttack)
                ? _resources.LightAttackStaminaCost
                : 0; // heavy оплатим на старте charge

            if (!HasStaminaOrAssumeFullOnSpawnTick(id, staminaCost))
                return;

            _attackCooldowns.ConsumeAttack(id, tick, cooldownTicks);

            var phases = (type == CombatActionType.HeavyAttack) ? _tuning.CombatActions.Heavy : _tuning.CombatActions.Light;
            SplitPhases(totalDurationTicks, phases, out var w, out var a, out var r);

            var inst = new CombatActionInstance(
                type: type,
                startTick: tick,
                windupTicks: w,
                activeTicks: a,
                recoveryTicks: r,
                cooldownTicks: cooldownTicks,
                lockedFacing: NormalizeFacing(facing),
                hitApplied: false);

            _actions.Set(id, inst);
            PublishEvent(id, tick, action, totalDurationTicks);

            SpendIfNeeded(id, staminaCost);
        }

        public void TryStartParry(GameEntityId id, int tick, int facing)
        {
            if (IsBusy(id, tick))
                return;

            var cfg = _tuning.CombatActions.Parry;
            if (!_combatCooldowns.CanUse(id, CombatActionType.Parry, tick))
                return;

            int staminaCost = _resources.ParryStaminaCost;
            if (!HasStaminaOrAssumeFullOnSpawnTick(id, staminaCost))
                return;

            _combatCooldowns.Consume(id, CombatActionType.Parry, tick, cfg.CooldownBaseTicks);
            StartFixed(id, tick, CombatActionType.Parry, ActionState.Parry, cfg, facing);

            SpendIfNeeded(id, staminaCost);
        }

        public void TryStartDodge(GameEntityId id, int tick, int direction)
        {
            bool hasCur = _actions.TryGet(id, out var cur) && cur.IsRunningAt(tick);
            if (hasCur)
            {
                if (cur.Type == CombatActionType.Dodge || cur.Type == CombatActionType.Parry)
                    return;

                if (!_cancelRules.CanCancel(in cur, CombatActionType.Dodge, tick))
                    return;
            }

            var cfg = _tuning.CombatActions.Dodge;
            if (!_combatCooldowns.CanUse(id, CombatActionType.Dodge, tick))
                return;

            int staminaCost = _resources.DodgeStaminaCost;
            if (!HasStaminaOrAssumeFullOnSpawnTick(id, staminaCost))
                return;

            _combatCooldowns.Consume(id, CombatActionType.Dodge, tick, cfg.CooldownBaseTicks);
            StartFixed(id, tick, CombatActionType.Dodge, ActionState.Dodge, cfg, direction);

            SpendIfNeeded(id, staminaCost);
        }

        private bool HasStaminaOrAssumeFullOnSpawnTick(GameEntityId id, int cost)
        {
            if (cost <= 0) return true;

            // На spawn-тик статы могут быть ещё не инициализированы (StatsInit будет в post-physics).
            // По факту там будет full stamina, так что разрешаем.
            if (!_stats.TryGet(id, out var s) || !s.IsInitialized)
                return true;

            return s.StaminaCur >= cost;
        }

        private void SpendIfNeeded(GameEntityId id, int cost)
        {
            if (cost <= 0) return;
            _deltas.SpendStamina(id, cost, StatsDeltaKind.Cost);
        }

        private bool IsBusy(GameEntityId id, int tick)
        {
            return _actions.TryGet(id, out var cur) && cur.IsRunningAt(tick);
        }

        private void StartFixed(GameEntityId id, int tick, CombatActionType type, ActionState action, CombatActionsTuning.FixedAction cfg, int facing)
        {
            int total = cfg.DurationBaseTicks;
            SplitPhases(total, cfg.Phases, out var w, out var a, out var r);

            var inst = new CombatActionInstance(
                type: type,
                startTick: tick,
                windupTicks: w,
                activeTicks: a,
                recoveryTicks: r,
                cooldownTicks: cfg.CooldownBaseTicks,
                lockedFacing: NormalizeFacing(facing),
                hitApplied: false);

            _actions.Set(id, inst);
            PublishEvent(id, tick, action, total);
        }

        private void PublishEvent(GameEntityId id, int tick, ActionState action, int durationTicks)
        {
            _events.SetTiming(id, action, durationTicks, tick);
            _events.SetIntent(id, action, tick);
        }

        private static sbyte NormalizeFacing(int facing) => (sbyte)(facing < 0 ? -1 : 1);

        private static CombatActionType ToCombatActionType(ActionState a)
        {
            if (a == ActionState.LightAttack) return CombatActionType.LightAttack;
            if (a == ActionState.HeavyAttack) return CombatActionType.HeavyAttack;
            if (a == ActionState.Parry) return CombatActionType.Parry;
            if (a == ActionState.Dodge) return CombatActionType.Dodge;
            return CombatActionType.None;
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