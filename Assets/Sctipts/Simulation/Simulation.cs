using System.Collections.Generic;
using Game.Domain.Abstractions;
using Game.Domain.Model;

namespace Game.Simulation
{

    public sealed class Simulation : ISimulation
    {
        private readonly GameState _state;
        private readonly ICommandDispatcher _dispatcher;

        public Simulation(GameState state, ICommandDispatcher dispatcher)
        {
            _state = state;
            _dispatcher = dispatcher;
        }

        public GameState State => _state;

        public void Step(IReadOnlyList<ICommand> commandsForTick)
        {
            _dispatcher.Dispatch(commandsForTick);
            _state.AdvanceTick();
        }
    }
}