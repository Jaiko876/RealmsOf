namespace Riftborne.App.Animation.Utils
{
    internal static class AnimationMath
    {
        internal const float Tiny = 0.0001f;

        public static float Normalize01(float value, float max)
        {
            if (max <= Tiny) return 0f;
            return Clamp01(value / max);
        }

        public static float ApplyDeadZone01(float v, float eps)
        {
            if (v < eps) return 0f;
            return v;
        }

        /// <summary>
        /// Returns 0..1 where 0..0.5 is ascending, 0.5..1 descending.
        /// </summary>
        public static float ComputeAirT(float vy, float jumpVel, float maxFall)
        {
            if (jumpVel < Tiny) jumpVel = Tiny;
            if (maxFall < Tiny) maxFall = Tiny;

            if (vy >= 0f)
            {
                var u = Clamp01(vy / jumpVel);
                return 0.5f * (1f - u);
            }

            var d = Clamp01((-vy) / maxFall);
            return 0.5f + 0.5f * d;
        }

        public static float Clamp01(float v)
        {
            if (v < 0f) return 0f;
            if (v > 1f) return 1f;
            return v;
        }
    }
}