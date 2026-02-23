using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Core.Physics.Model;

namespace Riftborne.Core.Physics.Provider
{
    public sealed class NullPhysicsModifiersProvider : IPhysicsModifiersProvider
    {
        public PhysicsModifiers Get(GameEntityId entityId) => PhysicsModifiers.None;
    }
}