namespace Riftborne.Core.Model
{
    public sealed class PlayerState
    {
        public PlayerId Id { get; }

        public float X { get; private set; }
        public float Y { get; private set; }

        public float Vx { get; private set; }
        public float Vy { get; private set; }

        public PlayerState(PlayerId id)
        {
            Id = id;
        }

        // Legacy API (useful for non-physical movement / tests)
        public void Move(float dx, float dy)
        {
            X += dx;
            Y += dy;
        }

        public void SetPose(float x, float y, float vx, float vy)
        {
            X = x;
            Y = y;
            Vx = vx;
            Vy = vy;
        }
    }
}
