using System;

namespace Riftborne.Core.Model
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

        public int Facing { get; private set; } = 1;
        public int PrevFacing { get; private set; } = 1;
        
        public bool Grounded { get; private set; }
        public bool Moving { get; private set; }

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
            PrevFacing = Facing;
        }

        public void SetPose(float x, float y, float vx, float vy, bool grounded)
        {
            X = x;
            Y = y;
            Vx = vx;
            Vy = vy;
            Grounded = grounded;
            Moving = Math.Abs(vx) > 0.01f;

            if (vx > 0.0001f) Facing = 1;
            else if (vx < -0.0001f) Facing = -1;
        }
        
    }
}