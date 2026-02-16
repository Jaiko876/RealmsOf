using System;

namespace Game.App.Commands
{
    public interface ICommandHandlerRegistration
    {
        Type CommandType { get; }
        void Invoke(object command);
    }
}
