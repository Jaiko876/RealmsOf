using System.Collections.Generic;
using Riftborne.Core.Abstractions;

namespace Riftborne.App.Commands
{

    public interface ICommandQueue
    {
        void Enqueue(ICommand command);
        IReadOnlyList<ICommand> DequeueAllForTick(int tick);
    }
}