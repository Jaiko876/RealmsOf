using Riftborne.Core.Model;

namespace Riftborne.Core.Physics.Model
{
    public readonly struct MotorInput
    {
        public readonly GameEntityId EntityId;
        public readonly float MoveX;
        public readonly bool JumpPressed;
        public readonly bool JumpHeld;

        public MotorInput(GameEntityId entityId, float moveX, bool jumpPressed, bool jumpHeld)
        {
            EntityId = entityId;
            MoveX = moveX;
            JumpPressed = jumpPressed;
            JumpHeld = jumpHeld;
        }
        
        public static MotorInput None(GameEntityId id) => new(id, 0f, false, false);
    }
}
