using Riftborne.Core.Model;

namespace Riftborne.Core.Physics.Model
{
    public struct MotorState
    {
        public GameEntityId EntityId;
        public int CoyoteTicks;
        public int JumpBufferTicks;

        public MotorState(GameEntityId id)
        {
            EntityId = id;
            CoyoteTicks = 0;
            JumpBufferTicks = 0;
        }
    }
}