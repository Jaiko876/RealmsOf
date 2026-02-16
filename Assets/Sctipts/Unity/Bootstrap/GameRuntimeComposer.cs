using System;
using Game.App.Commands;
using Game.App.Combat;
using Game.App.Level;
using Game.Core.Abstractions;
using Game.Core.Level;
using Game.Core.Random;
using Game.Core.Simulation;
using Game.Core.Systems;
using Game.Core.Combat.Damage;
using Game.Core.Combat.Health;
using Game.Core.Combat.Resources;
using Game.Core.Combat.Abilities;
using Game.Core.Combat.Rules;
using Game.Core.Combat.Resolution;
using Game.Core.Stats;
using Game.Configs;
using Game.Physics.Unity2D;
using Game.Unity.Combat;
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
        private readonly CombatConfigAsset _combatConfig;

        private IScopedObjectResolver _runtimeScope;

        public GameRuntimeComposer(
            IObjectResolver root,
            SimulationParameters simParams,
            LevelSeed levelSeed,
            LevelGenConfig levelGenConfig,
            CombatConfigAsset combatConfigAsset
            )
        {
            _root = root;
            _simParams = simParams;
            _levelSeed = levelSeed;
            _levelGenConfig = levelGenConfig;
            _combatConfig = combatConfigAsset;
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

                // combat config
                builder.RegisterInstance(_combatConfig);

                // core combat tunings
                builder.RegisterInstance(_combatConfig.ToDamageTuning());
                builder.RegisterInstance(_combatConfig.ToRulesConfig());
                builder.RegisterInstance(_combatConfig.ToHitQueryTuning());

                // abilities from config
                builder.Register<IAbilityDefinitionProvider, ConfigAbilityDefinitionProvider>(Lifetime.Singleton);


                // --- Damage/Health ---
                builder.Register<IHealthStore, InMemoryHealthStore>(Lifetime.Singleton);
                builder.Register<IDamageCalculator, DefaultDamageCalculator>(Lifetime.Singleton);
                builder.Register<IHealthDamageService, HealthDamageService>(Lifetime.Singleton);
                // (опционально) health tick system
                builder.Register<IHealthTickSystem, HealthTickSystem>(Lifetime.Singleton);

                // --- Combat Resources ---
                builder.Register<ICombatResourceStore, InMemoryCombatResourceStore>(Lifetime.Singleton);
                builder.Register<ICombatResourceTickSystem, CombatResourceTickSystem>(Lifetime.Singleton);

                // --- Abilities ---
                builder.Register<ICombatActionStore, InMemoryCombatActionStore>(Lifetime.Singleton);
                builder.Register<IAbilitySystem, AbilitySystem>(Lifetime.Singleton);
                builder.Register<ICombatActionTickSystem, CombatActionTickSystem>(Lifetime.Singleton);

                builder.Register<ICombatRulesResolver, CombatRulesResolver>(Lifetime.Singleton);

                // --- Combat Resolution ---
                builder.Register<ICombatResolutionSystem, CombatResolutionSystem>(Lifetime.Singleton);
                builder.Register<IHitQuery, UnityPhysicsHitQuery>(Lifetime.Singleton);

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
                builder.Register<UseAbilityCommandHandler>(Lifetime.Singleton)
                    .As<ICommandHandler<Game.Core.Combat.Abilities.UseAbilityCommand>>();

                builder.Register<CommandHandlerRegistration<Game.Core.Combat.Abilities.UseAbilityCommand>>(Lifetime.Singleton)
                    .As<Game.App.Commands.ICommandHandlerRegistration>();


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
