using Riftborne.Core.Gameplay.Physics.Modifiers;
using Riftborne.Core.Gameplay.Physics.Providers;
using Riftborne.Core.Model;

namespace Riftborne.App.Physics.Providers        
{
    public sealed class NullPhysicsModifiersProvider : IPhysicsModifiersProvider
    {
        public PhysicsModifiers Get(GameEntityId entityId) => PhysicsModifiers.None;
    }
}