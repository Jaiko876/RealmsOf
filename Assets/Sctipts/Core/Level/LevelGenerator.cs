using System.Collections.Generic;

namespace Game.Core.Level
{
    public sealed class LevelGenerator
    {
        public LevelDefinition GenerateFlat(int width, int groundY)
        {
            var cells = new List<LevelCell>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y <= groundY; y++)
                {
                    cells.Add(new LevelCell(x, y));
                }
            }

            return new LevelDefinition(cells, width, groundY);
        }
    }
}
