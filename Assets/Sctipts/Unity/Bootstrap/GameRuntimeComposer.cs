using System;

using Cysharp.Threading.Tasks;
using Game.Core.Abstractions;
using Game.Core.Random;
using Game.Core.Simulation;
using Game.Core.Systems;
using Game.Core.Level;
using Game.App.Commands;
using Game.Unity.Config;
using VContainer;
using VContainer.Unity;
using UnityEngine;

using Game.Physics.Unity2D;
using Game.Physics;

namespace Game.Unity.Bootstrap
{
    public sealed class GameRuntimeComposer : IStartable, IDisposable
    {
        private readonly IObjectResolver _root;
        private readonly IGameConfigProvider _configProvider;

        private IScopedObjectResolver _runtimeScope;

        public GameRuntimeComposer(IObjectResolver root, IGameConfigProvider configProvider)
        {
            _root = root;
            _configProvider = configProvider;
        }

        public void Start()
        {
            InitializeAsync().Forget();
        }

        public void Dispose()
        {
            if (_runtimeScope != null)
            {
                _runtimeScope.Dispose();
                _runtimeScope = null;
            }
        }

        private async UniTaskVoid InitializeAsync()
        {
            var config = await _configProvider.LoadAsync();

            Time.fixedDeltaTime = 1f / config.TickRate;

            Physics2DScriptBootstrap.EnsureScriptMode();

            _runtimeScope = _root.CreateScope(builder =>
            {
                var dt = 1f / config.TickRate;

                builder.RegisterInstance(new SimulationParameters(
                    unitsPerTick: config.UnitsPerTick,
                    tickDeltaTime: dt,
                    physicsSubsteps: 1
                ));

                // --- Physics (Unity2D backend) ---
                builder.Register<Unity2DPhysicsWorld>(Lifetime.Singleton)
                    .As<Game.Core.Physics.Abstractions.IPhysicsWorld>();

                builder.Register<Unity2DGroundSensor>(Lifetime.Singleton)
                    .As<Game.Core.Physics.Abstractions.IGroundSensor>();

                builder.Register<Game.Physics.PlatformerCharacterMotor>(Lifetime.Singleton)
                    .As<Game.Core.Physics.Abstractions.ICharacterMotor>();

                // TODO: подцепи из config реальные значения
                builder.RegisterInstance(new Game.Core.Physics.Model.MotorParams());

                builder.Register<Game.Physics.Registry.BodyRegistry>(Lifetime.Singleton)
                    .As<Game.Core.Physics.Abstractions.IBodyProvider<Game.Core.Model.GameEntityId>>();

                builder.RegisterComponentInHierarchy<Game.Physics.Unity2D.PhysicsBodyAuthoring>();

                // handlers
                builder.Register<MoveCommandHandler>(Lifetime.Singleton)
                    .As<ICommandHandler<Game.Core.Commands.MoveCommand>>();

                // registrations
                builder.Register<CommandHandlerRegistration<Game.Core.Commands.MoveCommand>>(Lifetime.Singleton)
                    .As<Game.App.Commands.ICommandHandlerRegistration>();

                // dispatcher
                builder.Register<Game.App.Commands.CommandDispatcher>(Lifetime.Singleton)
                    .As<Game.Core.Abstractions.ICommandDispatcher>();

                // Simulation
                builder.Register<ISimulation, Simulation>(Lifetime.Singleton);

                // Game loop
                builder.RegisterEntryPoint<GameLoop>();

                // RAng
                builder.Register<Game.Core.Random.XorShiftRandomFactory>(Lifetime.Singleton)
                       .As<Game.Core.Abstractions.IRandomFactory>();

                // Level
                builder.Register<LevelGenerator>(Lifetime.Singleton);

                builder.RegisterComponentInHierarchy<Game.Unity.Level.TilemapLevelView>()
                       .As<Game.App.Level.ILevelView>();

                builder.RegisterComponentInHierarchy<Game.Unity.Spawning.UnitySpawnSystem>()
                       .As<Game.Core.Spawning.ISpawnSystem>();

                builder.Register<Game.App.Level.LevelService>(Lifetime.Singleton);


            });


            var level = _runtimeScope.Resolve<Game.App.Level.LevelService>();
            level.StartLevel();
        }
    }
}
