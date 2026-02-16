using System.Collections.Generic;

namespace Riftborne.Core.Abstractions
{

    public interface ICommandDispatcher
    {
        void Dispatch(IReadOnlyList<ICommand> commands);
    }
}
