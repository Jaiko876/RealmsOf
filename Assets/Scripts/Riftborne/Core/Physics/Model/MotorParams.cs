namespace Riftborne.Core.Physics.Model
{
    public sealed class MotorParams
    {
        public float MaxSpeedX { get; }
        public float AccelX { get; }
        public float DecelX { get; }
        public float JumpVelocity { get; }
        public int CoyoteTicks { get; }
        public int JumpBufferTicks { get; }

        public MotorParams(
            float maxSpeedX,
            float accelX,
            float decelX,
            float jumpVelocity,
            int coyoteTicks,
            int jumpBufferTicks)
        {
            MaxSpeedX = maxSpeedX;
            AccelX = accelX;
            DecelX = decelX;
            JumpVelocity = jumpVelocity;
            CoyoteTicks = coyoteTicks;
            JumpBufferTicks = jumpBufferTicks;
        }
    }
}