using System;

namespace Game.Core.Level
{
    internal sealed class HeightmapPass : ILevelGenPass
    {
        private enum SegmentKind
        {
            Plains,
            Hills,
            Mountains
        }

        public void Apply(LevelGenContext ctx)
        {
            var c = ctx.Config;

            int y = Clamp(c.BaseGroundY, c.MinGroundY, c.MaxGroundY);

            int x = 0;
            while (x < c.Width)
            {
                var kind = PickSegmentKind(ctx, c);
                int len = ctx.Rng.NextInt(c.MinSegmentLen, c.MaxSegmentLen + 1);
                if (len < 1) len = 1;

                int step = StepFor(kind, c);
                for (int i = 0; i < len && x < c.Width; i++, x++)
                {
                    // random walk
                    int delta = 0;
                    if (step > 0)
                    {
                        delta = ctx.Rng.NextInt(-step, step + 1);
                    }

                    // Ограничиваем “проходимость” прямо тут: slope clamp
                    delta = Clamp(delta, -c.MaxStepDown, c.MaxStepUp);

                    y = Clamp(y + delta, c.MinGroundY, c.MaxGroundY);
                    ctx.GroundY[x] = y;
                }
            }
        }

        private static SegmentKind PickSegmentKind(LevelGenContext ctx, LevelGenConfig c)
        {
            int total = c.PlainsChance + c.HillsChance + c.MountainsChance;
            if (total <= 0) return SegmentKind.Plains;

            int r = ctx.Rng.NextInt(0, total);
            if (r < c.PlainsChance) return SegmentKind.Plains;
            r -= c.PlainsChance;
            if (r < c.HillsChance) return SegmentKind.Hills;
            return SegmentKind.Mountains;
        }

        private static int StepFor(SegmentKind kind, LevelGenConfig c)
        {
            switch (kind)
            {
                case SegmentKind.Plains: return c.PlainsStep;
                case SegmentKind.Hills: return c.HillsStep;
                case SegmentKind.Mountains: return c.MountainsStep;
                default: return 1;
            }
        }

        private static int Clamp(int v, int min, int max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }
    }
}
