using System.Collections.Generic;
using Game.Core.Abstractions;
using Game.Core.Model;

namespace Game.Core.Simulation
{

    public interface ISimulation
    {
        GameState State { get; }
        void Step(int tick, IReadOnlyList<ICommand> commandsForTick);
    }
}