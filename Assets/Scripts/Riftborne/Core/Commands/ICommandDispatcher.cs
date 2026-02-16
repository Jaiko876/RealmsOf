using System.Collections.Generic;

namespace Riftborne.Core.Commands
{

    public interface ICommandDispatcher
    {
        void Dispatch(IReadOnlyList<ICommand> commands);
    }
}
