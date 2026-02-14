using System.Collections.Generic;
using Game.Core.Abstractions;

namespace Game.App.Commands
{

    public interface ICommandQueue
    {
        void Enqueue(ICommand command);
        IReadOnlyList<ICommand> DequeueAllForTick(int tick);
    }
}