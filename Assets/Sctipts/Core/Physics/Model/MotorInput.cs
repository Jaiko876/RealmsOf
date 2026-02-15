using Game.Core.Model;

namespace Game.Core.Physics.Model
{
    public readonly struct MotorInput
    {
        public readonly PlayerId PlayerId;
        public readonly float MoveX;      // [-1..1]
        public readonly bool JumpPressed; // edge
        public readonly bool JumpHeld;

        public MotorInput(
            PlayerId playerId,
            float moveX,
            bool jumpPressed,
            bool jumpHeld)
        {
            PlayerId = playerId;
            MoveX = moveX;
            JumpPressed = jumpPressed;
            JumpHeld = jumpHeld;
        }
    }
}
