using System;
using Riftborne.App.Combat.Abstractions;
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

        public EvadeInputCommandHandler(GameState state, ICombatActionStarter starter)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _starter = starter ?? throw new ArgumentNullException(nameof(starter));
        }

        public void Handle(InputCommand command)
        {
            if ((command.Buttons & InputButtons.EvadePressed) == 0)
                return;

            if (!_state.Entities.TryGetValue(command.EntityId, out var e) || e == null)
                return;

            _starter.TryStartDodge(command.EntityId, command.Tick, e.Facing);
        }
    }
}