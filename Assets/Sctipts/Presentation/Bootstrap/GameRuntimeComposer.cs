using System;

using Cysharp.Threading.Tasks;
using Game.Domain.Abstractions;
using Game.Domain.Random;
using Game.Simulation;
using Game.Infrastructure.Commands;
using Game.Infrastructure.Config;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace Game.Presentation.Bootstrap
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

            // Настраиваем Unity fixedDeltaTime под tick rate (важно для стабильного тика)
            Time.fixedDeltaTime = 1f / config.TickRate;

            _runtimeScope = _root.CreateScope(builder =>
            {
                builder.RegisterInstance(new SimulationParameters(config.UnitsPerTick));
                builder.Register<IRandomSource>(_ => new XorShiftRandomSource(config.Seed), Lifetime.Singleton);

                // Register handlers
                builder.Register<MoveCommandHandler>(Lifetime.Singleton)
                        .As<ICommandHandler<Game.Domain.Commands.MoveCommand>>();

                // Simulation
                builder.Register<ISimulation, Simulation.Simulation>(Lifetime.Singleton);

                // Game loop
                builder.RegisterEntryPoint<GameLoop>();
            });
        }
    }
}