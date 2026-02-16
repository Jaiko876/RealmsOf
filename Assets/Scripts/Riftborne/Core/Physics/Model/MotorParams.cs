namespace Riftborne.Core.Physics.Model
{

    /// <summary>
    /// Platformer feel parameters (pure data, configurable from GameConfig later).
    /// </summary>
    public sealed class MotorParams
    {
        public float MaxSpeedX = 8f;
        public float AccelX = 60f;
        public float DecelX = 70f;

        public float JumpVelocity = 12f;

        // Feel helpers
        public float CoyoteTimeSeconds = 0.08f;
        public float JumpBufferSeconds = 0.10f;
    }
}
