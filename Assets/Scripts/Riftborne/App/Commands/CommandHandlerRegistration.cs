using System;
using Riftborne.Core.Commands;

namespace Riftborne.App.Commands
{
    public sealed class CommandHandlerRegistration<TCommand> : ICommandHandlerRegistration
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;

        public CommandHandlerRegistration(ICommandHandler<TCommand> handler)
        {
            _handler = handler;
        }

        public Type CommandType
        {
            get { return typeof(TCommand); }
        }

        public void Invoke(object command)
        {
            // Hotpath: только cast + вызов
            _handler.Handle((TCommand)command);
        }
    }
}
