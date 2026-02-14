using Game.Core.Abstractions;
using Game.Core.Commands;
using Game.Core.Model;
using Game.Core.Simulation;

namespace Game.Core.Systems
{

    public sealed class MoveCommandHandler : ICommandHandler<MoveCommand>
    {
        private readonly GameState _state;
        private readonly SimulationParameters _parameters;

        public MoveCommandHandler(GameState state, SimulationParameters parameters)
        {
            _state = state;
            _parameters = parameters;
        }

        public void Handle(MoveCommand command)
        {
            var player = _state.GetOrCreatePlayer(command.PlayerId);
            player.Move(command.Dx * _parameters.UnitsPerTick, command.Dy * _parameters.UnitsPerTick);
        }
    }
}