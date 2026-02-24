using Riftborne.App.Commands.Handlers;
using Riftborne.App.Spawning;
using Riftborne.App.Spawning.Abstractions;
using Riftborne.App.Spawning.Handlers;
using Riftborne.App.Spawning.Hooks;
using Riftborne.App.Spawning.Lifecycle;
using Riftborne.App.Spawning.Lifecycle.Abstractions;
using Riftborne.Core.Input.Commands;
using VContainer;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class SpawningRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 250;

        public void Initialize(IContainerBuilder builder)
        {
            // ids: старт с 1000, чтобы не конфликтовать с тем, что руками в сцене
            builder.Register<IEntityIdAllocator>(c => new SequentialEntityIdAllocator(1000), Lifetime.Singleton);

            builder.Register<IEntityLifecycle, EntityLifecycle>(Lifetime.Singleton);

            // command handlers + registrations
            builder.Register<SpawnEntityCommandHandler>(Lifetime.Singleton);
            builder.Register<DespawnEntityCommandHandler>(Lifetime.Singleton);
            
            //hooks
            builder.Register<IEntityLifecycleHook, PlayerAvatarCleanupHook>(Lifetime.Singleton);
            builder.Register<IEntityLifecycleHook, StoresCleanupHook>(Lifetime.Singleton);
            builder.Register<IEntityLifecycleHook, BodyRegistryCleanupHook>(Lifetime.Singleton);

            builder.Register<ICommandHandlerRegistration, CommandHandlerRegistration<SpawnEntityCommand, SpawnEntityCommandHandler>>(Lifetime.Singleton);
            builder.Register<ICommandHandlerRegistration, CommandHandlerRegistration<DespawnEntityCommand, DespawnEntityCommandHandler>>(Lifetime.Singleton);
        }
    }
}