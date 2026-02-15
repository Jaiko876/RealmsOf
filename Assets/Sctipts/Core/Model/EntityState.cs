namespace Game.Core.Model
{
    public sealed class EntityState
    {
        public GameEntityId Id { get; }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Vx { get; private set; }
        public float Vy { get; private set; }

        public float PrevX { get; private set; }
        public float PrevY { get; private set; }
        public float PrevVx { get; private set; }
        public float PrevVy { get; private set; }

        public EntityState(GameEntityId id)
        {
            Id = id;
        }

        public void BeginTick()
        {
            PrevX = X;
            PrevY = Y;
            PrevVx = Vx;
            PrevVy = Vy;
        }

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
