using Riftborne.Core.Model;

namespace Riftborne.Core.Physics.Model
{
    public readonly struct MotorInput
    {
        public readonly GameEntityId EntityId;
        public readonly float MoveX;
        public readonly bool JumpPressed;
        public readonly bool JumpHeld;
        
        public readonly sbyte FacingIntent;

        public MotorInput(GameEntityId entityId, float moveX, bool jumpPressed, bool jumpHeld, sbyte facingIntent)
        {
            EntityId = entityId;
            MoveX = moveX;
            JumpPressed = jumpPressed;
            JumpHeld = jumpHeld;
            FacingIntent = facingIntent;
        }

        public static MotorInput None(GameEntityId id)
        {
            return new MotorInput(id, 0f, false, false, 0);
        }
    }
}