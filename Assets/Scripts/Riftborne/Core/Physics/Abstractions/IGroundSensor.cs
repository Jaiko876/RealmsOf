using Riftborne.Core.Model;

namespace Riftborne.Core.Physics.Abstractions
{
    public interface IGroundSensor
    {
        bool IsGrounded(GameEntityId id);
    }
}
