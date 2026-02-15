using Game.App.Commands;
using Game.App.Time;
using Game.Core.Abstractions;
using Game.Core.Level;
using Game.Core.Model;
using Game.Core.Simulation;
using Game.Unity.Input;
using Game.Unity.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Unity.Bootstrap
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private Game.Configs.GameConfigAsset _gameConfig;
        [SerializeField] private Game.Configs.LevelGenConfigAsset _levelConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            // --- Core infra (живёт весь матч) ---
            builder.Register<FixedTickClock>(Lifetime.Singleton).As<ITickClock>();
            builder.Register<InMemoryCommandQueue>(Lifetime.Singleton).As<ICommandQueue>();

            // --- Domain state (живёт весь матч) ---
            builder.Register<GameState>(Lifetime.Singleton);

            // --- Configs (из инспектора) ---
            builder.RegisterInstance(_gameConfig);

            // Simulation parameters (из game config)
            var dt = 1f / Mathf.Max(1, _gameConfig.TickRate);
            builder.RegisterInstance(new SimulationParameters(
                unitsPerTick: _gameConfig.UnitsPerTick,
                tickDeltaTime: dt,
                physicsSubsteps: 1
            ));

            // Level seed/config (из инспектора)
            builder.RegisterInstance(new LevelSeed(_gameConfig.Seed));

            builder.RegisterInstance(new LevelGenConfig
            {
                Width = _levelConfig.Width,
                BaseGroundY = _levelConfig.BaseGroundY,
                MinGroundY = _levelConfig.MinGroundY,
                MaxGroundY = _levelConfig.MaxGroundY,
                MaxStepUp = _levelConfig.MaxStepUp,
                MaxStepDown = _levelConfig.MaxStepDown,
                MinSegmentLen = _levelConfig.MinSegmentLen,
                MaxSegmentLen = _levelConfig.MaxSegmentLen,
                PlainsStep = _levelConfig.PlainsStep,
                HillsStep = _levelConfig.HillsStep,
                MountainsStep = _levelConfig.MountainsStep,
                PlainsChance = _levelConfig.PlainsChance,
                HillsChance = _levelConfig.HillsChance,
                MountainsChance = _levelConfig.MountainsChance,
                FillDepth = _levelConfig.FillDepth,
                StoneStartDepth = _levelConfig.StoneStartDepth,
                SpawnerCount = _levelConfig.SpawnerCount,
                SpawnerMinDistanceFromPlayer = _levelConfig.SpawnerMinDistanceFromPlayer
            });

            // --- Entry point: соберёт runtime scope и запустит матч ---
            builder.RegisterEntryPoint<GameRuntimeComposer>();

            // --- Scene components (в root, чтобы composer мог их найти) ---
            builder.RegisterComponentInHierarchy<PlayerView>();
            builder.RegisterComponentInHierarchy<PlayerInputController>();
            builder.RegisterComponentInHierarchy<PlayerInputAdapter>();
        }
    }
}
