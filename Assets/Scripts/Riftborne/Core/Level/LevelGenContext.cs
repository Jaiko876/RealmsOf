using System.Collections.Generic;
using Riftborne.Core.Random;

namespace Riftborne.Core.Level
{
    public sealed class LevelGenContext
    {
        public LevelGenConfig Config { get; }
        public IRandomSource Rng { get; }

        // Heightmap: groundY per X
        public int[] GroundY { get; }

        // Output buffers
        public List<LevelTile> Tiles { get; } = new List<LevelTile>(4096);
        public List<LevelCell> SolidCells { get; } = new List<LevelCell>(4096);
        public List<LevelObjectSpawn> Objects { get; } = new List<LevelObjectSpawn>(32);

        public LevelGenContext(LevelGenConfig config, IRandomSource rng)
        {
            Config = config;
            Rng = rng;
            GroundY = new int[config.Width];
        }

        public LevelDefinition BuildDefinition()
        {
            int minY = 0;
            int maxY = 0;
            for (int i = 0; i < GroundY.Length; i++)
            {
                if (i == 0)
                {
                    minY = GroundY[i];
                    maxY = GroundY[i];
                }
                else
                {
                    if (GroundY[i] < minY) minY = GroundY[i];
                    if (GroundY[i] > maxY) maxY = GroundY[i];
                }
            }

            return new LevelDefinition(
                tiles: Tiles,
                objects: Objects,
                solidCells: SolidCells,
                width: Config.Width,
                minY: minY,
                maxY: maxY);
        }
    }
}
