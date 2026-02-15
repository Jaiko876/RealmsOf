using System.Collections.Generic;

namespace Game.Core.Level
{
    public sealed class LevelDefinition
    {
        public IReadOnlyList<LevelCell> SolidCells { get; }
        public int Width { get; }
        public int GroundY { get; }

        public LevelDefinition(
            IReadOnlyList<LevelCell> solidCells,
            int width,
            int groundY)
        {
            SolidCells = solidCells;
            Width = width;
            GroundY = groundY;
        }
    }
}
