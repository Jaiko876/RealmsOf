using Riftborne.App.Commands.Handlers;
using Riftborne.App.Spawning;
using Riftborne.App.Spawning.Abstractions;
using Riftborne.App.Spawning.Handlers;
using Riftborne.App.Spawning.Hooks;
using Riftborne.App.Spawning.Hooks.Lifecycle;
using Riftborne.App.Spawning.Lifecycle;
using Riftborne.App.Spawning.Lifecycle.Abstractions;
using Riftborne.Core.Input.Commands;
using Riftborne.Unity.Spawning;
using VContainer;
using VContainer.Unity;

namespace Riftborne.Unity.Bootstrap.Runtime
{
    public sealed class SpawningRuntimeInitializer : IRuntimeInitializer
    {
        public int Order => 250;

        public void Initialize(IContainerBuilder builder)
        {
            // IMPORTANT: runtime-scope resolver (must be used for InjectGameObject of spawned prefabs)
            builder.RegisterComponentInHierarchy<UnitySpawnBackend>().As<ISpawnBackend>();

            builder.Register<IEntityIdAllocator>(c => new SequentialEntityIdAllocator(1000), Lifetime.Singleton);
            builder.Register<IEntityLifecycle, EntityLifecycle>(Lifetime.Singleton);

            builder.Register<SpawnEntityCommandHandler>(Lifetime.Singleton);
            builder.Register<DespawnEntityCommandHandler>(Lifetime.Singleton);
            builder.Register<SpawnPlayerAvatarCommandHandler>(Lifetime.Singleton);

            builder.Register<IEntityLifecycleHook, PlayerAvatarCleanupHook>(Lifetime.Singleton);
            builder.Register<IEntityLifecycleHook, StoresCleanupHook>(Lifetime.Singleton);
            builder.Register<IEntityLifecycleHook, BodyRegistryCleanupHook>(Lifetime.Singleton);

            builder.Register<ICommandHandlerRegistration, CommandHandlerRegistration<SpawnEntityCommand, SpawnEntityCommandHandler>>(Lifetime.Singleton);
            builder.Register<ICommandHandlerRegistration, CommandHandlerRegistration<DespawnEntityCommand, DespawnEntityCommandHandler>>(Lifetime.Singleton);
            builder.Register<ICommandHandlerRegistration, CommandHandlerRegistration<SpawnPlayerAvatarCommand, SpawnPlayerAvatarCommandHandler>>(Lifetime.Singleton);
        }
    }
}