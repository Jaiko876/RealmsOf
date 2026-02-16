namespace Riftborne.Core.Physics.Model
{
    public sealed class MotorParams
    {
        public float MaxSpeedX { get; }
        public float AccelX { get; }
        public float DecelX { get; }
        public float JumpVelocity { get; }
        public float CoyoteTimeSeconds { get; }
        public float JumpBufferSeconds { get; }

        public MotorParams(
            float maxSpeedX,
            float accelX,
            float decelX,
            float jumpVelocity,
            float coyoteTimeSeconds,
            float jumpBufferSeconds)
        {
            MaxSpeedX = maxSpeedX;
            AccelX = accelX;
            DecelX = decelX;
            JumpVelocity = jumpVelocity;
            CoyoteTimeSeconds = coyoteTimeSeconds;
            JumpBufferSeconds = jumpBufferSeconds;
        }
    }
}