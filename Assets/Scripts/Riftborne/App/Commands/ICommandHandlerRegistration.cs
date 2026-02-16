using System;

namespace Riftborne.App.Commands
{
    public interface ICommandHandlerRegistration
    {
        Type CommandType { get; }
        void Invoke(object command);
    }
}
