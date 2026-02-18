using Riftborne.Core.Model;

namespace Riftborne.Core.Physics.Abstractions
{
    public interface IWallSensor
    {
        bool IsBlockedLeft(GameEntityId entityId);
        bool IsBlockedRight(GameEntityId entityId);
    }
}