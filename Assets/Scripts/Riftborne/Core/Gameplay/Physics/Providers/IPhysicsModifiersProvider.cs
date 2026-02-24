using Riftborne.Core.Gameplay.Physics.Modifiers;
using Riftborne.Core.Model;

namespace Riftborne.Core.Gameplay.Physics.Providers
{
    public interface IPhysicsModifiersProvider
    {
        PhysicsModifiers Get(GameEntityId entityId);
    }
}