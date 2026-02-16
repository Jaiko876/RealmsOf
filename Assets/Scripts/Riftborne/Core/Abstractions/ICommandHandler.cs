namespace Riftborne.Core.Abstractions
{

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        void Handle(TCommand command);
    }
}