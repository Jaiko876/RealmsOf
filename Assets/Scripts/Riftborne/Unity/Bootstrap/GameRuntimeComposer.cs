using System;
using Riftborne.App.Combat;
using Riftborne.App.Commands;
using Riftborne.App.Level;
using Riftborne.Configs;
using Riftborne.Core.Abstractions;
using Riftborne.Core.Combat.Abilities;
using Riftborne.Core.Combat.Damage;
using Riftborne.Core.Combat.Health;
using Riftborne.Core.Combat.Resolution;
using Riftborne.Core.Combat.Resources;
using Riftborne.Core.Combat.Rules;
using Riftborne.Core.Commands;
using Riftborne.Core.Factory;
using Riftborne.Core.Level;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Core.Physics.Model;
using Riftborne.Core.Physics.Registry;
using Riftborne.Core.Simulation;
using Riftborne.Core.Spawning;
using Riftborne.Core.Stats;
using Riftborne.Core.Systems;
using Riftborne.Physics;
using Riftborne.Physics.Unity2D;
using Riftborne.Unity.Combat;
using Riftborne.Unity.Spawning;
using Riftborne.Unity.View.Level;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Riftborne.Unity.Bootstrap
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
                builder.RegisterInstance(_combatConfig.ToResourceTuning());

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
                    .As<IPhysicsWorld>();

                builder.Register<Unity2DGroundSensor>(Lifetime.Singleton)
                    .As<IGroundSensor>();

                builder.Register<PlatformerCharacterMotor>(Lifetime.Singleton)
                    .As<ICharacterMotor>();

                // TODO: позже вынесешь в эдитор-конфиг
                builder.RegisterInstance(new MotorParams());

                builder.Register<BodyRegistry>(Lifetime.Singleton)
                    .As<IBodyProvider<GameEntityId>>();

                builder.RegisterComponentInHierarchy<PhysicsBodyAuthoring>();

                // --- Commands: handlers + dispatcher ---
                builder.Register<MoveCommandHandler>(Lifetime.Singleton)
                    .As<ICommandHandler<MoveCommand>>();
                builder.Register<UseAbilityCommandHandler>(Lifetime.Singleton)
                    .As<ICommandHandler<UseAbilityCommand>>();

                builder.Register<CommandHandlerRegistration<UseAbilityCommand>>(Lifetime.Singleton)
                    .As<ICommandHandlerRegistration>();


                builder.Register<CommandHandlerRegistration<MoveCommand>>(Lifetime.Singleton)
                    .As<ICommandHandlerRegistration>();

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

                builder.RegisterComponentInHierarchy<TilemapLevelView>()
                    .As<ILevelView>();

                builder.RegisterComponentInHierarchy<UnitySpawnSystem>()
                    .As<ISpawnSystem>();

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
