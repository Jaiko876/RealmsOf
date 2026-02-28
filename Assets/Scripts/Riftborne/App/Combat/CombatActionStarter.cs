// Assets/Scripts/Riftborne/App/Combat/CombatActionStarter.cs

using System;
using Riftborne.App.Combat.Abstractions;
using Riftborne.Core.Config;
using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.App.Combat
{
    public sealed class CombatActionStarter : ICombatActionStarter
    {
        private readonly ICombatActionStore _actions;
        private readonly IActionEventStore _events;
        private readonly IAttackCooldownStore _attackCooldowns;
        private readonly IGameplayTuning _tuning;

        public CombatActionStarter(
            ICombatActionStore actions,
            IActionEventStore events,
            IAttackCooldownStore attackCooldowns,
            IGameplayTuning tuning)
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _events = events ?? throw new ArgumentNullException(nameof(events));
            _attackCooldowns = attackCooldowns ?? throw new ArgumentNullException(nameof(attackCooldowns));
            _tuning = tuning ?? throw new ArgumentNullException(nameof(tuning));
        }

        public void TryStartAttack(GameEntityId id, int tick, ActionState action, int totalDurationTicks, int cooldownTicks, int facing)
        {
            var type = ToCombatActionType(action);
            if (type == CombatActionType.None)
                return;

            // no overlap (v1). cancel rules придут следующим шагом.
            if (_actions.TryGet(id, out var cur) && cur.IsRunningAt(tick))
                return;

            if (!_attackCooldowns.CanAttack(id, tick))
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
                lockedFacing: (sbyte)(facing < 0 ? -1 : 1),
                hitApplied: false);

            _actions.Set(id, inst);

            // publish animation event
            _events.SetTiming(id, action, totalDurationTicks, tick);
            _events.SetIntent(id, action, tick);
        }

        public void TryStartParry(GameEntityId id, int tick, int facing)
        {
            var cfg = _tuning.CombatActions.Parry;
            TryStartFixed(id, tick, CombatActionType.Parry, ActionState.Parry, cfg, facing);
        }

        public void TryStartDodge(GameEntityId id, int tick, int facing)
        {
            var cfg = _tuning.CombatActions.Dodge;
            TryStartFixed(id, tick, CombatActionType.Dodge, ActionState.Dodge, cfg, facing);
        }

        private void TryStartFixed(GameEntityId id, int tick, CombatActionType type, ActionState action, CombatActionsTuning.FixedAction cfg, int facing)
        {
            if (_actions.TryGet(id, out var cur) && cur.IsRunningAt(tick))
                return;

            int total = cfg.DurationBaseTicks;
            SplitPhases(total, cfg.Phases, out var w, out var a, out var r);

            var inst = new CombatActionInstance(
                type: type,
                startTick: tick,
                windupTicks: w,
                activeTicks: a,
                recoveryTicks: r,
                cooldownTicks: cfg.CooldownBaseTicks,
                lockedFacing: (sbyte)(facing < 0 ? -1 : 1),
                hitApplied: false);

            _actions.Set(id, inst);

            _events.SetTiming(id, action, total, tick);
            _events.SetIntent(id, action, tick);
        }

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

            // proportional split with exact sum == totalTicks
            windup = (int)MathF.Round(totalTicks * (w.WindupWeight / (float)sum));
            active = (int)MathF.Round(totalTicks * (w.ActiveWeight / (float)sum));

            if (windup < 0) windup = 0;
            if (active < 0) active = 0;

            int used = windup + active;
            if (used > totalTicks)
            {
                // clamp
                int overflow = used - totalTicks;
                if (active >= overflow) active -= overflow;
                else windup = Math.Max(0, windup - (overflow - active));
            }

            recovery = totalTicks - (windup + active);
            if (recovery < 0) recovery = 0;
        }
    }
}