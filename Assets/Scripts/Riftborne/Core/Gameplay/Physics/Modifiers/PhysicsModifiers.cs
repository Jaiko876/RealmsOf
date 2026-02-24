namespace Riftborne.Core.Gameplay.Physics.Modifiers
{
    /// <summary>
    /// "Chaos layer" output for this tick. Extend freely (wind, slime, gravity, etc).
    /// </summary>
    public readonly struct PhysicsModifiers
    {
        public readonly float GravityScaleMultiplier; // 1 = normal
        public readonly float ImpulseX;
        public readonly float ImpulseY;

        // Movement multipliers (1 = normal)
        public readonly float MoveSpeedMultiplier;
        public readonly float AccelMultiplier;
        public readonly float DecelMultiplier;

        public PhysicsModifiers(
            float gravityScaleMultiplier,
            float impulseX,
            float impulseY,
            float moveSpeedMultiplier,
            float accelMultiplier,
            float decelMultiplier)
        {
            GravityScaleMultiplier = gravityScaleMultiplier <= 0f ? 1f : gravityScaleMultiplier;
            ImpulseX = impulseX;
            ImpulseY = impulseY;

            MoveSpeedMultiplier = moveSpeedMultiplier <= 0f ? 1f : moveSpeedMultiplier;
            AccelMultiplier = accelMultiplier <= 0f ? 1f : accelMultiplier;
            DecelMultiplier = decelMultiplier <= 0f ? 1f : decelMultiplier;
        }

        // Backward-compatible ctor
        public PhysicsModifiers(float gravityScaleMultiplier, float impulseX, float impulseY)
            : this(gravityScaleMultiplier, impulseX, impulseY, 1f, 1f, 1f)
        {
        }

        public static PhysicsModifiers None => new PhysicsModifiers(1f, 0f, 0f, 1f, 1f, 1f);
    }
}