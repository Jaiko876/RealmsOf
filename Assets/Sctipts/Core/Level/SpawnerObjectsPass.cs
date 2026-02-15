namespace Game.Core.Level
{
    internal sealed class SpawnerObjectsPass : ILevelGenPass
    {
        public void Apply(LevelGenContext ctx)
        {
            var c = ctx.Config;

            // Игрок сейчас спавнится в LevelService. Для “не рядом с игроком”
            // считаем стартовую позицию игрока как X=5.
            int playerX = 5;
            int count = c.SpawnerCount;
            if (count < 0) count = 0;

            for (int i = 0; i < count; i++)
            {
                int x = PickX(ctx, c, playerX);
                int y = ctx.GroundY[x] + 1;

                ctx.Objects.Add(new LevelObjectSpawn("Spawner", x, y));
            }
        }

        private static int PickX(LevelGenContext ctx, LevelGenConfig c, int playerX)
        {
            // Простая попытка найти место подальше от игрока
            int tries = 32;
            int minDist = c.SpawnerMinDistanceFromPlayer;
            if (minDist < 0) minDist = 0;

            int best = ctx.Rng.NextInt(0, c.Width);

            for (int t = 0; t < tries; t++)
            {
                int x = ctx.Rng.NextInt(0, c.Width);
                int dist = x - playerX;
                if (dist < 0) dist = -dist;

                if (dist >= minDist)
                    return x;

                best = x;
            }

            return best;
        }
    }
}
