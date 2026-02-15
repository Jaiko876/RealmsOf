using System;

using Cysharp.Threading.Tasks;
using Game.Core.Abstractions;
using Game.Core.Random;
using Game.Core.Simulation;
using Game.Core.Systems;
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

                builder.Register<IRandomSource>(_ => new XorShiftRandomSource(config.Seed), Lifetime.Singleton);

                // --- Physics (Unity2D backend) ---
                builder.Register<Unity2DPhysicsWorld>(Lifetime.Singleton)
                    .As<Game.Core.Physics.Abstractions.IPhysicsWorld>();

                builder.Register<PlayerBodyRegistry>(Lifetime.Singleton)
                    .As<Game.Core.Physics.Abstractions.IPlayerBodyProvider>();

                builder.Register<Unity2DGroundSensor>(Lifetime.Singleton)
                    .As<Game.Core.Physics.Abstractions.IGroundSensor>();

                builder.Register<Game.Physics.PlatformerCharacterMotor>(Lifetime.Singleton)
                    .As<Game.Core.Physics.Abstractions.ICharacterMotor>();

                // TODO: подцепи из config реальные значения
                builder.RegisterInstance(new Game.Core.Physics.Model.MotorParams());

                // где-то рядом с остальными Register... внутри runtime scope
                builder.RegisterComponentInHierarchy<Game.Physics.Unity2D.PlayerPhysicsAuthoring>();


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
            });
        }
    }
}
