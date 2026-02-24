using System.Collections.Generic;
using Riftborne.Core.Input.Commands.Abstractions;

namespace Riftborne.App.Commands.Queue
{

    public interface ICommandQueue
    {
        void Enqueue(ICommand command);
        IReadOnlyList<ICommand> DequeueAllForTick(int tick);
    }
}