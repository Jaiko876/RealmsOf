using Riftborne.Core.Input.Commands.Abstractions;

namespace Riftborne.Core.Input.Handlers
{

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        void Handle(TCommand command);
    }
}