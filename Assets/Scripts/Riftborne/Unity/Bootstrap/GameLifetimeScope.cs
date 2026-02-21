using Riftborne.App.Commands;
using Riftborne.App.Time.Time;
using Riftborne.Configs;
using Riftborne.Core.Model;
using Riftborne.Core.Simulation;
using Riftborne.Unity.Bootstrap.Runtime;
using Riftborne.Unity.Input;
using Riftborne.Unity.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Riftborne.Unity.Bootstrap
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameConfigAsset _gameConfig;
        [SerializeField] private MotorConfigAsset _motorConfig;
        [SerializeField] private StatsConfigAsset _statsConfig;
        

        protected override void Configure(IContainerBuilder builder)
        {
            // --- Core infra (живёт весь матч) ---
            builder.Register<FixedTickClock>(Lifetime.Singleton).As<ITickClock>();
            builder.Register<InMemoryCommandQueue>(Lifetime.Singleton).As<ICommandQueue>();

            // --- Domain state (живёт весь матч) ---
            builder.Register<GameState>(Lifetime.Singleton);

            // --- Configs (из инспектора) ---
            if (_gameConfig == null) throw new System.InvalidOperationException("GameConfigAsset is not assigned in GameLifetimeScope.");
            if (_motorConfig == null) throw new System.InvalidOperationException("MotorConfigAsset is not assigned in GameLifetimeScope.");
            if (_statsConfig == null) throw new System.InvalidOperationException("StatsConfigAsset is not assigned in GameLifetimeScope.");
           
            builder.RegisterInstance(_statsConfig);
            builder.RegisterInstance(_motorConfig);
            builder.RegisterInstance(_gameConfig);

            // Simulation parameters (из game config)
            var dt = 1f / Mathf.Max(1, _gameConfig.TickRate);
            builder.RegisterInstance(new SimulationParameters(
                unitsPerTick: _gameConfig.UnitsPerTick,
                tickDeltaTime: dt,
                physicsSubsteps: 1
            ));

            // --- Entry point: соберёт runtime scope и запустит матч ---
            builder.Register<SpawningRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            builder.Register<StatsRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            builder.Register<PhysicRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            builder.Register<SystemsRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            builder.Register<CommandsRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            builder.Register<SimulationRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            builder.Register<RandomRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            builder.Register<StoresRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            
            builder.RegisterEntryPoint<GameRuntimeComposer>();
            
            // --- Scene components (в root, чтобы composer мог их найти) ---
            builder.RegisterComponentInHierarchy<PlayerView>();
            builder.RegisterComponentInHierarchy<PlayerInputController>();
            builder.RegisterComponentInHierarchy<PlayerInputAdapter>();
        }
    }
}
