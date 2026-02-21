// Assets/Scripts/Riftborne/Core/Spawning/Hooks/BodyRegistryCleanupHook.cs
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;

namespace Riftborne.Core.Spawning.Hook
{
    public sealed class BodyRegistryCleanupHook : EntityLifecycleHookBase
    {
        private readonly IBodyProvider<GameEntityId> _bodies;

        public BodyRegistryCleanupHook(IBodyProvider<GameEntityId> bodies) => _bodies = bodies;

        public override void OnAfterDespawn(GameEntityId id)
        {
            _bodies.Unregister(id);
        }
    }
}