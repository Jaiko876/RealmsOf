using Game.Core.Model;

namespace Game.Core.Physics.Abstractions
{
    public interface IGroundSensor
    {
        bool IsGrounded(GameEntityId id);
    }
}
