using Riftborne.App.Commands;
using Riftborne.App.Time.Time;
using Riftborne.Configs;
using Riftborne.Core.Model;
using Riftborne.Core.Simulation;
using Riftborne.Core.Spawning;
using Riftborne.Unity.Bootstrap.Runtime;
using Riftborne.Unity.Debugging;
using Riftborne.Unity.Input;
using Riftborne.Unity.Spawning;
using Riftborne.Unity.UI;
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
        [SerializeField] private GameplayTuningAsset _tuningConfig;
        

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
            if (_tuningConfig == null) throw new System.InvalidOperationException("GameplayTuningAsset is not assigned in GameLifetimeScope.");
           
            builder.RegisterInstance(_statsConfig);
            builder.RegisterInstance(_motorConfig);
            builder.RegisterInstance(_gameConfig);
            builder.RegisterInstance(_tuningConfig);

            // Simulation parameters (из game config)
            var dt = 1f / Mathf.Max(1, _gameConfig.TickRate);
            builder.RegisterInstance(new SimulationParameters(
                unitsPerTick: _gameConfig.UnitsPerTick,
                tickDeltaTime: dt,
                physicsSubsteps: 1
            ));
            
            builder.RegisterComponentInHierarchy<UnitySpawnBackend>().As<ISpawnBackend>();

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
        }
    }
}
