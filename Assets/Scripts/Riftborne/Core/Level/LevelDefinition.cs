using System.Collections.Generic;

namespace Riftborne.Core.Level
{
    public sealed class LevelDefinition
    {
        public IReadOnlyList<LevelTile> Tiles { get; }
        public IReadOnlyList<LevelObjectSpawn> Objects { get; }

        // Back-compat: “solid cells” для простых мест
        public IReadOnlyList<LevelCell> SolidCells { get; }

        public int Width { get; }
        public int MinY { get; }
        public int MaxY { get; }

        public LevelDefinition(
            IReadOnlyList<LevelTile> tiles,
            IReadOnlyList<LevelObjectSpawn> objects,
            IReadOnlyList<LevelCell> solidCells,
            int width,
            int minY,
            int maxY)
        {
            Tiles = tiles;
            Objects = objects;
            SolidCells = solidCells;
            Width = width;
            MinY = minY;
            MaxY = maxY;
        }
    }
}
