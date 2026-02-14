using System.Collections.Generic;

namespace Game.Domain.Abstractions
{

    public interface ICommandDispatcher
    {
        void Dispatch(IReadOnlyList<ICommand> commands);
    }
}
