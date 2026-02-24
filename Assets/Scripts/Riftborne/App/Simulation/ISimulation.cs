using System.Collections.Generic;
using Riftborne.Core.Input.Commands.Abstractions;
using Riftborne.Core.Model;

namespace Riftborne.App.Simulation
{

    public interface ISimulation
    {
        GameState State { get; }
        void Step(int tick, IReadOnlyList<ICommand> commandsForTick);
    }
}