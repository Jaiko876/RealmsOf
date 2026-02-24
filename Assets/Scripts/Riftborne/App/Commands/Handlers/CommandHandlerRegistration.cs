using System;
using Riftborne.Core.Input.Commands.Abstractions;
using Riftborne.Core.Input.Handlers;

namespace Riftborne.App.Commands.Handlers
{
    public sealed class CommandHandlerRegistration<TCommand, THandler> : ICommandHandlerRegistration
        where TCommand : ICommand
        where THandler : ICommandHandler<TCommand>
    {
        private readonly THandler _handler;

        public CommandHandlerRegistration(THandler handler)
        {
            _handler = handler;
        }

        public Type CommandType
        {
            get { return typeof(TCommand); }
        }

        public void Invoke(object command)
        {
            _handler.Handle((TCommand)command);
        }
    }
}