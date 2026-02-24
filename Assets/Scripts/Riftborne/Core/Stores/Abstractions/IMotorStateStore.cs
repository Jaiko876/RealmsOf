using Riftborne.Core.Model;
using Riftborne.Core.Physics.Model;

namespace Riftborne.Core.Stores.Abstractions
{
    public interface IMotorStateStore
    {
        MotorState GetOrCreate(GameEntityId id);
        void Set(MotorState state);
        void Remove(GameEntityId id);
    }
}