using Riftborne.Core.Model;
using Riftborne.Core.Physics.Model;

namespace Riftborne.Core.Input
{
    public interface IMotorInputStore
    {
        void Set(GameEntityId id, MotorInput input);
        bool TryGet(GameEntityId id, out MotorInput input);
        void Clear();
    }
}