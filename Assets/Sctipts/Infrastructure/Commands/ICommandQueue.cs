using System.Collections.Generic;
using Game.Domain.Abstractions;

namespace Game.Infrastructure.Commands
{

    public interface ICommandQueue
    {
        void Enqueue(ICommand command);
        IReadOnlyList<ICommand> DequeueAllForTick(int tick);
    }
}