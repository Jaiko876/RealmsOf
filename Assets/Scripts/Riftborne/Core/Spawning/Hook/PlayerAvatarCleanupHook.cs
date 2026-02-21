using Riftborne.Core.Model;

namespace Riftborne.Core.Spawning.Hook
{
    public sealed class PlayerAvatarCleanupHook : EntityLifecycleHookBase
    {
        private readonly GameState _state;

        public PlayerAvatarCleanupHook(GameState state) => _state = state;

        public override void OnAfterDespawn(GameEntityId id)
        {
            _state.PlayerAvatars.RemoveByEntity(id);
        }
    }
}