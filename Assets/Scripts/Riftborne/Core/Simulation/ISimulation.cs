using System.Collections.Generic;
using Riftborne.Core.Abstractions;
using Riftborne.Core.Model;

namespace Riftborne.Core.Simulation
{

    public interface ISimulation
    {
        GameState State { get; }
        void Step(int tick, IReadOnlyList<ICommand> commandsForTick);
    }
}