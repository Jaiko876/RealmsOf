using System;
using Riftborne.App.Combat.Abstractions;
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
        private readonly GameState _state;
        
        private readonly IAttackChargeStore _charge;
        private readonly IAttackHoldStore _hold;
        private readonly IAttackInputRules _rules;
        private readonly ICombatSpeedProvider _speeds;

        private readonly CombatInputTuning _inputTuning;
        private readonly CombatAnimationTuning _animTuning;
        
        private readonly ICombatActionStarter _starter;

        public ActionInputCommandHandler(
            IAttackChargeStore charge,
            IAttackHoldStore hold,
            IAttackInputRules rules,
            ICombatSpeedProvider speeds,
            IGameplayTuning gameplayTuning, 
            ICombatActionStarter starter, 
            GameState state)
        {
            _charge = charge ?? throw new ArgumentNullException(nameof(charge));
            _hold = hold ?? throw new ArgumentNullException(nameof(hold));
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _speeds = speeds ?? throw new ArgumentNullException(nameof(speeds));
            _starter = starter ?? throw new ArgumentNullException(nameof(starter));
            _state = state ?? throw new ArgumentNullException(nameof(state));
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

            // вместо прямого cooldown+events:
            if (_state.Entities.TryGetValue(id, out var e) && e != null)
            {
                _starter.TryStartAttack(id, tick, step.Release.Action, step.Release.DurationTicks, step.Release.CooldownTicks, e.Facing);
            }
        }
    }
}