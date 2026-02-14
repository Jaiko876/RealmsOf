using System.Collections.Generic;

namespace Game.Core.Abstractions
{

    public interface ICommandDispatcher
    {
        void Dispatch(IReadOnlyList<ICommand> commands);
    }
}
