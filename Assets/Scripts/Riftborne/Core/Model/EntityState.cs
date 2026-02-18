using System;
using Riftborne.Core.Model.Animation;

namespace Riftborne.Core.Model
{
    public sealed class EntityState
    {
        public GameEntityId Id { get; }
        public AnimationState AnimationState { get; private set; }

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
        public bool PrevGrounded { get; private set; }

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
            PrevGrounded = Grounded;
        }

        public void SetPose(float x, float y, float vx, float vy, bool grounded)
        {
            X = x;
            Y = y;
            Vx = vx;
            Vy = vy;
            Grounded = grounded;
        }

        public void SetAnimationState(AnimationState state) => AnimationState = state;
        
        public void ApplyFacingIntent(sbyte intent)
        {
            if (intent == 0) return;
            Facing = intent < 0 ? -1 : 1;
        }

    }
}