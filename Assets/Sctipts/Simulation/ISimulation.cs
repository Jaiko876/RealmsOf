using System.Collections.Generic;
using Game.Domain.Abstractions;
using Game.Domain.Model;

namespace Game.Simulation
{

    public interface ISimulation
    {
        GameState State { get; }
        void Step(IReadOnlyList<ICommand> commandsForTick);
    }
}