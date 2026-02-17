using Riftborne.App.Commands;
using Riftborne.Core.Commands;
using Riftborne.Core.Input;
using VContainer;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class CommandsRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 400;

        public void Initialize(IContainerBuilder builder)
        {
            builder.Register<ICommandHandler<InputCommand>, InputCommandHandler>(Lifetime.Singleton);
            builder.Register<ICommandHandlerRegistration, CommandHandlerRegistration<InputCommand>>(Lifetime.Singleton);
            builder.Register<ICommandDispatcher, CommandDispatcher>(Lifetime.Singleton);
        }
    }
}