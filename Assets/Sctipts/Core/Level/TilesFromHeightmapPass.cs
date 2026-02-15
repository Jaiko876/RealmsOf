namespace Game.Core.Level
{
    internal sealed class TilesFromHeightmapPass : ILevelGenPass
    {
        public void Apply(LevelGenContext ctx)
        {
            var c = ctx.Config;

            for (int x = 0; x < c.Width; x++)
            {
                int groundY = ctx.GroundY[x];

                // Верхний слой
                AddSolid(ctx, x, groundY, LevelTileKind.GrassTop);

                // Заполнение вниз: Dirt -> Stone
                for (int d = 1; d <= c.FillDepth; d++)
                {
                    int y = groundY - d;
                    var kind = d >= c.StoneStartDepth ? LevelTileKind.Stone : LevelTileKind.Dirt;
                    AddSolid(ctx, x, y, kind);
                }
            }
        }

        private static void AddSolid(LevelGenContext ctx, int x, int y, LevelTileKind kind)
        {
            ctx.Tiles.Add(new LevelTile(x, y, kind));
            ctx.SolidCells.Add(new LevelCell(x, y));
        }
    }
}
