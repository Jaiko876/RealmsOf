using System;

namespace Riftborne.App.Commands.Handlers
{
    public interface ICommandHandlerRegistration
    {
        Type CommandType { get; }
        void Invoke(object command);
    }
}
