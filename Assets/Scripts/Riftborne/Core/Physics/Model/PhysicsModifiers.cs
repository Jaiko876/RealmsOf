namespace Riftborne.Core.Physics.Model
{

    /// <summary>
    /// "Chaos layer" output for this tick. Extend freely (wind, slime, gravity, etc).
    /// </summary>
    public readonly struct PhysicsModifiers
    {
        public readonly float GravityScaleMultiplier; // 1 = normal
        public readonly float ImpulseX;
        public readonly float ImpulseY;

        public PhysicsModifiers(float gravityScaleMultiplier, float impulseX, float impulseY)
        {
            GravityScaleMultiplier = gravityScaleMultiplier <= 0f ? 1f : gravityScaleMultiplier;
            ImpulseX = impulseX;
            ImpulseY = impulseY;
        }

        public static PhysicsModifiers None => new(1f, 0f, 0f);
    }
}
