using System;
using Riftborne.Core.Config;
using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Gameplay.Combat.Rules.Abstractions;
using Riftborne.Core.Input;
using Riftborne.Core.Input.Commands;
using Riftborne.Core.Input.Handlers;
using Riftborne.Core.Model;
using Riftborne.Core.Stats;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.App.Input.Handlers
{
    public sealed class ActionInputCommandHandler : ICommandHandler<InputCommand>
    {
        private readonly IActionEventStore _events;
        private readonly IAttackChargeStore _charge;
        private readonly IAttackHoldStore _hold;
        private readonly IStatsStore _stats;
        private readonly IAttackCooldownStore _cooldowns;
        private readonly IAttackInputRules _rules;

        private readonly CombatInputTuning _inputTuning;
        private readonly CombatAnimationTuning _animTuning;

        public ActionInputCommandHandler(
            IActionEventStore events,
            IAttackChargeStore charge,
            IAttackHoldStore hold,
            IStatsStore stats,
            IAttackCooldownStore cooldowns,
            IAttackInputRules rules,
            IGameplayTuning gameplayTuning)
        {
            _events = events ?? throw new ArgumentNullException(nameof(events));
            _charge = charge ?? throw new ArgumentNullException(nameof(charge));
            _hold = hold ?? throw new ArgumentNullException(nameof(hold));
            _stats = stats ?? throw new ArgumentNullException(nameof(stats));
            _cooldowns = cooldowns ?? throw new ArgumentNullException(nameof(cooldowns));
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            if (gameplayTuning == null) throw new ArgumentNullException(nameof(gameplayTuning));

            _inputTuning = gameplayTuning.CombatInput;
            _animTuning = gameplayTuning.CombatAnimation;
        }

        public void Handle(InputCommand command)
        {
            GameEntityId id = command.EntityId;
            int tick = command.Tick;

            bool heldNow = (command.Buttons & InputButtons.AttackHeld) != 0;

            bool prevHeld;
            int heldTicks;
            if (!_hold.TryGet(id, out prevHeld, out heldTicks))
            {
                prevHeld = false;
                heldTicks = 0;
            }

            float attackSpeed = GetStatClamped(id, StatId.AttackSpeed, 1f, _inputTuning.MinAttackSpeed, _inputTuning.MaxAttackSpeed);
            float chargeSpeed = GetStatClamped(id, StatId.ChargeSpeed, 1f, _inputTuning.MinChargeSpeed, _inputTuning.MaxChargeSpeed);

            var req = new AttackInputStepRequest(
                id, tick, heldNow,
                prevHeld, heldTicks,
                attackSpeed, chargeSpeed,
                _inputTuning, _animTuning);

            AttackInputStep step = _rules.Step(in req);

            _hold.Set(id, step.Hold.IsHeld, step.Hold.HeldTicks);
            _charge.Set(id, step.Charge.Charging, step.Charge.Charge01);

            if (step.Release.HasRelease)
            {
                if (_cooldowns.CanAttack(id, tick))
                {
                    _cooldowns.ConsumeAttack(id, tick, step.Release.CooldownTicks);
                    _events.SetTiming(id, step.Release.Action, step.Release.DurationTicks, tick);
                    _events.SetIntent(id, step.Release.Action, tick);
                }
            }
        }

        private float GetStatClamped(GameEntityId id, StatId stat, float fallback, float min, float max)
        {
            float v = fallback;

            if (_stats.TryGet(id, out var s) && s.IsInitialized)
                v = s.GetEffective(stat);

            if (v < min) return min;
            if (v > max) return max;
            return v;
        }
    }
}