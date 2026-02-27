using System;
using Riftborne.App.Commands.Queue;
using Riftborne.App.Spawning.Abstractions;
using Riftborne.App.Time;
using Riftborne.Configs;
using Riftborne.Core.Config;
using Riftborne.Core.Model;
using Riftborne.Core.TIme;
using Riftborne.Unity.Bootstrap.Runtime;
using Riftborne.Unity.Input;
using Riftborne.Unity.Spawning;
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
        [SerializeField] private AttackAnimationConfigAsset _attackAnimConfig;
        

        protected override void Configure(IContainerBuilder builder)
        {
            // --- Core infra (живёт весь матч) ---
            builder.Register<FixedTickClock>(Lifetime.Singleton).As<ITickClock>();
            builder.Register<InMemoryCommandQueue>(Lifetime.Singleton).As<ICommandQueue>();

            // --- Domain state (живёт весь матч) ---
            builder.Register<GameState>(Lifetime.Singleton);

            // --- Configs (из инспектора) ---
            if (_gameConfig == null) throw new InvalidOperationException("GameConfigAsset is not assigned in GameLifetimeScope.");
            if (_motorConfig == null) throw new InvalidOperationException("MotorConfigAsset is not assigned in GameLifetimeScope.");
            if (_statsConfig == null) throw new InvalidOperationException("StatsConfigAsset is not assigned in GameLifetimeScope.");
            if (_tuningConfig == null) throw new InvalidOperationException("GameplayTuningAsset is not assigned in GameLifetimeScope.");
            if (_attackAnimConfig == null) throw new InvalidOperationException("AttackAnimationConfigAsset is not assigned in GameLifetimeScope.");
           
            builder.RegisterInstance(_statsConfig);
            builder.RegisterInstance(_motorConfig);
            builder.RegisterInstance(_gameConfig);
            builder.RegisterInstance(_tuningConfig).As<IGameplayTuning>(); 
            builder.RegisterInstance(_attackAnimConfig);

            var dt = 1f / Mathf.Max(1, _gameConfig.TickRate);
            builder.RegisterInstance(new SimulationParameters(
                unitsPerTick: _gameConfig.UnitsPerTick,
                tickDeltaTime: dt,
                physicsSubsteps: Mathf.Max(1, _tuningConfig.PhysicsWorld.MaxSubSteps) // <-- из tuning
            ));
            
            builder.RegisterComponentInHierarchy<PlayerInputController>();
            builder.RegisterComponentInHierarchy<PlayerInputAdapter>();
            
            builder.RegisterComponentInHierarchy<UnitySpawnBackend>().As<ISpawnBackend>();

            // --- Entry point: соберёт runtime scope и запустит матч ---
            builder.Register<CombatRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            builder.Register<AnimationsRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            builder.Register<SpawningRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            builder.Register<StatsRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            builder.Register<PhysicRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            builder.Register<SystemsRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            builder.Register<CommandsRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            builder.Register<SimulationRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            builder.Register<RandomRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            builder.Register<StoresRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            builder.Register<UnityRuntimeInitializer>(Lifetime.Singleton).As<IRuntimeInitializer>();
            
            builder.RegisterEntryPoint<GameRuntimeComposer>();
        }
    }
}
