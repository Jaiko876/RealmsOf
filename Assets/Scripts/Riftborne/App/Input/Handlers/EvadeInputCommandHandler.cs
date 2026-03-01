using System;
using Riftborne.App.Combat.Abstractions;
using Riftborne.Core.Config;
using Riftborne.Core.Input;
using Riftborne.Core.Input.Commands;
using Riftborne.Core.Input.Handlers;
using Riftborne.Core.Model;

namespace Riftborne.App.Input.Handlers
{
    public sealed class EvadeInputCommandHandler : ICommandHandler<InputCommand>
    {
        private readonly GameState _state;
        private readonly ICombatActionStarter _starter;
        private readonly float _deadzone;

        public EvadeInputCommandHandler(GameState state, ICombatActionStarter starter, IGameplayTuning tuning)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _starter = starter ?? throw new ArgumentNullException(nameof(starter));
            if (tuning == null) throw new ArgumentNullException(nameof(tuning));
            _deadzone = tuning.Input.FacingDeadzone;
        }

        public void Handle(InputCommand command)
        {
            if ((command.Buttons & InputButtons.EvadePressed) == 0)
                return;

            if (!_state.Entities.TryGetValue(command.EntityId, out var e) || e == null)
                return;

            int dir;
            if (command.Dx > _deadzone) dir = 1;
            else if (command.Dx < -_deadzone) dir = -1;
            else dir = e.Facing; // no "dodge in place": default to facing

            _starter.TryStartDodge(command.EntityId, command.Tick, dir);
        }
    }
}