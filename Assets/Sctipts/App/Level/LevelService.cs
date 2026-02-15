using Game.Core.Level;
using Game.Core.Spawning;

namespace Game.App.Level
{
    public sealed class LevelService
    {
        private readonly LevelGenerator _generator;
        private readonly ILevelView _view;
        private readonly ISpawnSystem _spawner;

        public LevelService(
            LevelGenerator generator,
            ILevelView view,
            ISpawnSystem spawner)
        {
            _generator = generator;
            _view = view;
            _spawner = spawner;
        }

        public void StartLevel()
        {
            var def = _generator.GenerateFlat(
                width: 200,
                groundY: 5);

            _view.Build(def);

            // Спавним игрока
            _spawner.Spawn(new SpawnRequest(
                type: "Player",
                x: 5,
                y: def.GroundY + 2));
        }
    }
}
