using System.Collections.Generic;
using Riftborne.Core.Commands;
using Riftborne.Core.Model;

namespace Riftborne.Core.Simulation
{

    public interface ISimulation
    {
        GameState State { get; }
        void Step(int tick, IReadOnlyList<ICommand> commandsForTick);
    }
}