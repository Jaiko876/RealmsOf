using System.Collections.Generic;
using Riftborne.Core.Commands;

namespace Riftborne.App.Commands
{

    public interface ICommandQueue
    {
        void Enqueue(ICommand command);
        IReadOnlyList<ICommand> DequeueAllForTick(int tick);
    }
}