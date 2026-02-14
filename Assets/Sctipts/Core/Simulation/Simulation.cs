using System.Collections.Generic;
using Game.Core.Abstractions;
using Game.Core.Model;

namespace Game.Core.Simulation
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

        public void Step(int tick, IReadOnlyList<ICommand> commandsForTick)
        {
            _dispatcher.Dispatch(commandsForTick);
            // Синхронизируем tick состояния с "источником времени" (GameLoop/Clock)
            _state.SetTick(tick + 1);
        }
    }
}