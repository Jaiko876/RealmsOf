using Game.Core.Abstractions;
using Game.Core.Model;
using Game.Core.Simulation;
using Game.App.Commands;
using Game.Unity.Config;
using Game.Unity.Input;
using Game.Unity.View;
using Game.App.Time;
using VContainer;
using VContainer.Unity;


namespace Game.Unity.Bootstrap
{

    public sealed class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Core infra
            builder.Register<FixedTickClock>(Lifetime.Singleton).As<ITickClock>();
            builder.Register<InMemoryCommandQueue>(Lifetime.Singleton).As<ICommandQueue>();

            // Config
            builder.Register<IGameConfigProvider>(_ => new AddressablesGameConfigProvider("GameConfig"), Lifetime.Singleton);

            // Domain state (живёт весь матч)
            builder.Register<GameState>(Lifetime.Singleton);

            // Dispatcher + handlers (handlers можно регать до загрузки конфига,
            // но сами они зависят от SimulationParameters — это будет в runtime scope)

            builder.RegisterEntryPoint<GameRuntimeComposer>();

            builder.RegisterComponentInHierarchy<PlayerView>();
            builder.RegisterComponentInHierarchy<PlayerInputController>();
            builder.RegisterComponentInHierarchy<PlayerInputAdapter>();

        }
    }
}