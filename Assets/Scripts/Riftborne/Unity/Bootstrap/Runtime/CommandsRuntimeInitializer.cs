using Riftborne.App.Commands.Dispatching;
using Riftborne.App.Commands.Handlers;
using Riftborne.App.Input.Handlers;
using Riftborne.Core.Input.Commands;
using VContainer;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class CommandsRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 1400;

        public void Initialize(IContainerBuilder builder)
        {
            // Конкретные обработчики (без регистрации одного и того же ICommandHandler<> два раза)
            builder.Register<InputCommandHandler>(Lifetime.Singleton);
            builder.Register<ActionInputCommandHandler>(Lifetime.Singleton);

            // ДВЕ разные реализации ICommandHandlerRegistration (уникальный implementation type)
            builder.Register<ICommandHandlerRegistration, CommandHandlerRegistration<InputCommand, InputCommandHandler>>(Lifetime.Singleton);
            builder.Register<ICommandHandlerRegistration, CommandHandlerRegistration<InputCommand, ActionInputCommandHandler>>(Lifetime.Singleton);
            
            // Assets/Scripts/Riftborne/Unity/Bootstrap/Runtime/CommandsRuntimeInitializer.cs
            builder.Register<DefenseHoldInputCommandHandler>(Lifetime.Singleton);
            builder.Register<EvadeInputCommandHandler>(Lifetime.Singleton);

            builder.Register<ICommandHandlerRegistration, CommandHandlerRegistration<InputCommand, DefenseHoldInputCommandHandler>>(Lifetime.Singleton);
            builder.Register<ICommandHandlerRegistration, CommandHandlerRegistration<InputCommand, EvadeInputCommandHandler>>(Lifetime.Singleton);

            builder.Register<ICommandDispatcher, CommandDispatcher>(Lifetime.Singleton);
        }
    }
}