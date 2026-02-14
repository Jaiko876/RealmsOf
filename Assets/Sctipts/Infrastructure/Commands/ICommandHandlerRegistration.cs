using System;

namespace Game.Infrastructure.Commands
{
    public interface ICommandHandlerRegistration
    {
        Type CommandType { get; }
        void Invoke(object command);
    }
}
