using System;
using Riftborne.App.Combat.Providers.Abstractions;
using Riftborne.Core.Config;
using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Gameplay.Combat.Rules.Abstractions;
using Riftborne.Core.Input;
using Riftborne.Core.Input.Commands;
using Riftborne.Core.Input.Handlers;
using Riftborne.Core.Model;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.App.Input.Handlers
{
    public sealed class ActionInputCommandHandler : ICommandHandler<InputCommand>
    {
        private readonly IActionEventStore _events;
        private readonly IAttackChargeStore _charge;
        private readonly IAttackHoldStore _hold;
        private readonly IAttackCooldownStore _cooldowns;
        private readonly IAttackInputRules _rules;
        private readonly ICombatSpeedProvider _speeds;

        private readonly CombatInputTuning _inputTuning;
        private readonly CombatAnimationTuning _animTuning;

        public ActionInputCommandHandler(
            IActionEventStore events,
            IAttackChargeStore charge,
            IAttackHoldStore hold,
            IAttackCooldownStore cooldowns,
            IAttackInputRules rules,
            ICombatSpeedProvider speeds,
            IGameplayTuning gameplayTuning)
        {
            _events = events ?? throw new ArgumentNullException(nameof(events));
            _charge = charge ?? throw new ArgumentNullException(nameof(charge));
            _hold = hold ?? throw new ArgumentNullException(nameof(hold));
            _cooldowns = cooldowns ?? throw new ArgumentNullException(nameof(cooldowns));
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _speeds = speeds ?? throw new ArgumentNullException(nameof(speeds));
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

            var speeds = _speeds.Get(id);

            var req = new AttackInputStepRequest(
                id, tick, heldNow,
                prevHeld, heldTicks,
                speeds.AttackSpeed, speeds.ChargeSpeed,
                _inputTuning, _animTuning);

            AttackInputStep step = _rules.Step(in req);

            _hold.Set(id, step.Hold.IsHeld, step.Hold.HeldTicks);
            _charge.Set(id, step.Charge.Charging, step.Charge.Charge01);

            if (!step.Release.HasRelease)
                return;

            if (!_cooldowns.CanAttack(id, tick))
                return;

            _cooldowns.ConsumeAttack(id, tick, step.Release.CooldownTicks);
            _events.SetTiming(id, step.Release.Action, step.Release.DurationTicks, tick);
            _events.SetIntent(id, step.Release.Action, tick);
        }
    }
}