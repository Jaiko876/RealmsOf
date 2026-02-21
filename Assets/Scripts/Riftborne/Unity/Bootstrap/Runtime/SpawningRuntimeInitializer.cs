using Riftborne.App.Commands;
using Riftborne.Core.Commands;
using Riftborne.Core.Spawning;
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
            // ids: старт с 1000, чтобы не конфликтовать с тем, что руками в сцене
            builder.Register<IEntityIdAllocator>(c => new SequentialEntityIdAllocator(1000), Lifetime.Singleton);

            // Unity backend: должен быть компонентом в сцене
            builder.RegisterComponentInHierarchy<UnitySpawnBackend>().As<ISpawnBackend>();

            builder.Register<IEntityLifecycle, EntityLifecycle>(Lifetime.Singleton);

            // command handlers + registrations
            builder.Register<SpawnEntityCommandHandler>(Lifetime.Singleton);
            builder.Register<DespawnEntityCommandHandler>(Lifetime.Singleton);

            builder.Register<ICommandHandlerRegistration, CommandHandlerRegistration<SpawnEntityCommand, SpawnEntityCommandHandler>>(Lifetime.Singleton);
            builder.Register<ICommandHandlerRegistration, CommandHandlerRegistration<DespawnEntityCommand, DespawnEntityCommandHandler>>(Lifetime.Singleton);
        }
    }
}