using Riftborne.App.Commands;
using Riftborne.Core.Commands;
using Riftborne.Core.Systems;
using VContainer;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class CommandsRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 300;

        public void Initialize(IContainerBuilder builder)
        {
            builder.Register<MoveCommandHandler>(Lifetime.Singleton).As<ICommandHandler<MoveCommand>>();
            builder.Register<CommandHandlerRegistration<MoveCommand>>(Lifetime.Singleton).As<ICommandHandlerRegistration>();
            builder.Register<CommandDispatcher>(Lifetime.Singleton).As<ICommandDispatcher>();
        }
    }
}