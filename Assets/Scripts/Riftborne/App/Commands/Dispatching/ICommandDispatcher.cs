using System.Collections.Generic;
using Riftborne.Core.Input.Commands.Abstractions;

namespace Riftborne.App.Commands.Dispatching
{

    public interface ICommandDispatcher
    {
        void Dispatch(IReadOnlyList<ICommand> commands);
    }
}
