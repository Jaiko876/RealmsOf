using System;
using Game.App.Commands;
using Game.App.Level;
using Game.Core.Abstractions;
using Game.Core.Level;
using Game.Core.Random;
using Game.Core.Simulation;
using Game.Core.Systems;
using Game.Core.Combat.Damage;
using Game.Core.Combat.Health;
using Game.Core.Combat.Resources;
using Game.Core.Stats;
using Game.Physics.Unity2D;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Unity.Bootstrap
{
    public sealed class GameRuntimeComposer : IStartable, IDisposable
    {
        private readonly IObjectResolver _root;
        private readonly SimulationParameters _simParams;
        private readonly LevelSeed _levelSeed;
        private readonly LevelGenConfig _levelGenConfig;

        private IScopedObjectResolver _runtimeScope;

        public GameRuntimeComposer(
            IObjectResolver root,
            SimulationParameters simParams,
            LevelSeed levelSeed,
            LevelGenConfig levelGenConfig)
        {
            _root = root;
            _simParams = simParams;
            _levelSeed = levelSeed;
            _levelGenConfig = levelGenConfig;
        }

        public void Start()
        {
            Initialize();
        }

        public void Dispose()
        {
            if (_runtimeScope != null)
            {
                _runtimeScope.Dispose();
                _runtimeScope = null;
            }
        }

        private void Initialize()
        {
            // фиксируем physics step под tick dt
            Time.fixedDeltaTime = _simParams.TickDeltaTime;

            Physics2DScriptBootstrap.EnsureScriptMode();

            _runtimeScope = _root.CreateScope(builder =>
            {
                // --- shared match/runtime params ---
                builder.RegisterInstance(_simParams);
                builder.RegisterInstance(_levelSeed);
                builder.RegisterInstance(_levelGenConfig);

                // --- Stats (core foundation) ---
                builder.Register<StatResolver>(Lifetime.Singleton);
                builder.Register<DefaultStatsSource>(Lifetime.Singleton);
                builder.Register<EntityBaseStatsSource>(Lifetime.Singleton);
                builder.Register<RuntimeModifiersSource>(Lifetime.Singleton);

                // --- Damage/Health ---
                builder.Register<DamageTuning>(Lifetime.Singleton);
                builder.Register<IHealthStore, InMemoryHealthStore>(Lifetime.Singleton);
                builder.Register<IDamageCalculator, DefaultDamageCalculator>(Lifetime.Singleton);
                builder.Register<IHealthDamageService, HealthDamageService>(Lifetime.Singleton);

                // --- Combat Resources ---
                builder.Register<ICombatResourceStore, InMemoryCombatResourceStore>(Lifetime.Singleton);
                builder.Register<ICombatResourceTickSystem, CombatResourceTickSystem>(Lifetime.Singleton);


                // (опционально) health tick system
                builder.Register<IHealthTickSystem, HealthTickSystem>(Lifetime.Singleton);

                // --- Physics (Unity2D backend) ---
                builder.Register<Unity2DPhysicsWorld>(Lifetime.Singleton)
                    .As<Game.Core.Physics.Abstractions.IPhysicsWorld>();

                builder.Register<Unity2DGroundSensor>(Lifetime.Singleton)
                    .As<Game.Core.Physics.Abstractions.IGroundSensor>();

                builder.Register<Game.Physics.PlatformerCharacterMotor>(Lifetime.Singleton)
                    .As<Game.Core.Physics.Abstractions.ICharacterMotor>();

                // TODO: позже вынесешь в эдитор-конфиг
                builder.RegisterInstance(new Game.Core.Physics.Model.MotorParams());

                builder.Register<Game.Physics.Registry.BodyRegistry>(Lifetime.Singleton)
                    .As<Game.Core.Physics.Abstractions.IBodyProvider<Game.Core.Model.GameEntityId>>();

                builder.RegisterComponentInHierarchy<Game.Physics.Unity2D.PhysicsBodyAuthoring>();

                // --- Commands: handlers + dispatcher ---
                builder.Register<MoveCommandHandler>(Lifetime.Singleton)
                    .As<ICommandHandler<Game.Core.Commands.MoveCommand>>();

                builder.Register<CommandHandlerRegistration<Game.Core.Commands.MoveCommand>>(Lifetime.Singleton)
                    .As<Game.App.Commands.ICommandHandlerRegistration>();

                builder.Register<CommandDispatcher>(Lifetime.Singleton)
                    .As<ICommandDispatcher>();

                // --- Simulation + loop ---
                builder.Register<ISimulation, Simulation>(Lifetime.Singleton);
                builder.RegisterEntryPoint<GameLoop>();

                // --- RNG ---
                builder.Register<XorShiftRandomFactory>(Lifetime.Singleton)
                    .As<IRandomFactory>();

                // --- Level ---
                builder.Register<LevelGenerator>(Lifetime.Singleton);

                builder.RegisterComponentInHierarchy<Game.Unity.Level.TilemapLevelView>()
                    .As<ILevelView>();

                builder.RegisterComponentInHierarchy<Game.Unity.Spawning.UnitySpawnSystem>()
                    .As<Game.Core.Spawning.ISpawnSystem>();

                builder.Register<LevelService>(Lifetime.Singleton);
            });

            // --- Init stat sources order ---
            var statResolver = _runtimeScope.Resolve<StatResolver>();
            var defaultStats = _runtimeScope.Resolve<DefaultStatsSource>();
            var baseStats = _runtimeScope.Resolve<EntityBaseStatsSource>();
            var runtimeMods = _runtimeScope.Resolve<RuntimeModifiersSource>();

            statResolver.ClearSources();
            statResolver.AddSource(defaultStats);
            statResolver.AddSource(baseStats);
            statResolver.AddSource(runtimeMods);


            // стартуем уровень
            var level = _runtimeScope.Resolve<LevelService>();
            level.StartLevel();
        }
    }
}
