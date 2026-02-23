using Riftborne.Core.Model;
using Riftborne.Core.Physics.Model;

namespace Riftborne.Core.Physics.Abstractions
{
    public interface IPhysicsModifiersProvider
    {
        PhysicsModifiers Get(GameEntityId entityId);
    }
}