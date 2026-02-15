using Game.Core.Level;
using Game.Core.Spawning;

namespace Game.App.Level
{
    public sealed class LevelService
    {
        private readonly LevelGenerator _generator;
        private readonly ILevelView _view;
        private readonly ISpawnSystem _spawner;

        private readonly LevelGenConfig _levelConfig;
        private readonly LevelSeed _seed;

        public LevelService(
            LevelGenerator generator,
            ILevelView view,
            ISpawnSystem spawner,
            LevelGenConfig levelConfig,
            LevelSeed seed)
        {
            _generator = generator;
            _view = view;
            _spawner = spawner;
            _levelConfig = levelConfig;
            _seed = seed;
        }

        public void StartLevel()
        {
            var def = _generator.Generate(_levelConfig, _seed);

            _view.Build(def);

            // Спавним игрока (как было)
            _spawner.Spawn(new SpawnRequest(
                type: "Player",
                x: 5,
                y: PickPlayerSpawnY(def, 5)));

            // Спавним объекты уровня (сейчас: Spawner)
            var objs = def.Objects;
            for (int i = 0; i < objs.Count; i++)
            {
                var o = objs[i];
                _spawner.Spawn(new SpawnRequest(o.Type, o.X, o.Y));
            }
        }

        private static int PickPlayerSpawnY(LevelDefinition def, int playerX)
        {
            // Дёшево/сердито: ищем поверхность в Tiles по x (GrassTop)
            // Если не нашли — fallback
            var tiles = def.Tiles;
            int bestY = def.MinY + 2;

            for (int i = 0; i < tiles.Count; i++)
            {
                var t = tiles[i];
                if (t.X == playerX && t.Kind == LevelTileKind.GrassTop)
                {
                    bestY = t.Y + 2;
                    break;
                }
            }
            return bestY;
        }
    }
}
