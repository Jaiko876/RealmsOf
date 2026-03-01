using System;
using Riftborne.App.Combat.Abstractions;
using Riftborne.Core.Config;
using Riftborne.Core.Input;
using Riftborne.Core.Input.Commands;
using Riftborne.Core.Input.Handlers;
using Riftborne.Core.Model;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.App.Input.Handlers
{
    public sealed class DefenseHoldInputCommandHandler : ICommandHandler<InputCommand>
    {
        private readonly GameState _state;
        private readonly IDefenseHoldStore _hold;
        private readonly IBlockStateStore _block;
        private readonly ICombatActionStarter _starter;
        private readonly DefenseInputTuning _tuning;

        public DefenseHoldInputCommandHandler(
            GameState state,
            IDefenseHoldStore hold,
            IBlockStateStore block,
            ICombatActionStarter starter,
            IGameplayTuning tuning)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _hold = hold ?? throw new ArgumentNullException(nameof(hold));
            _block = block ?? throw new ArgumentNullException(nameof(block));
            _starter = starter ?? throw new ArgumentNullException(nameof(starter));
            if (tuning == null) throw new ArgumentNullException(nameof(tuning));
            _tuning = tuning.DefenseInput;
        }

        public void Handle(InputCommand command)
        {
            GameEntityId id = command.EntityId;
            int tick = command.Tick;

            bool heldNow = (command.Buttons & InputButtons.DefenseHeld) != 0;

            bool prevHeld;
            int heldTicks;
            if (!_hold.TryGet(id, out prevHeld, out heldTicks))
            {
                prevHeld = false;
                heldTicks = 0;
            }

            // update hold
            bool started = heldNow && !prevHeld;
            if (started) heldTicks = 0;
            if (heldNow) heldTicks++;
            bool released = !heldNow && prevHeld;

            _hold.Set(id, heldNow, heldTicks);

            // block on hold after threshold
            if (heldNow && heldTicks >= _tuning.BlockStartTicks)
            {
                _block.SetBlocking(id, true);
            }
            else if (!heldNow)
            {
                _block.SetBlocking(id, false);
            }

            if (!released)
                return;

            // Tap => parry on release only if it was short and never reached block
            if (heldTicks <= _tuning.ParryMaxTapTicks)
            {
                if (_state.Entities.TryGetValue(id, out var e) && e != null)
                    _starter.TryStartParry(id, tick, e.Facing);
            }

            // reset after release
            _hold.Set(id, false, 0);
            _block.SetBlocking(id, false);
        }
    }
}