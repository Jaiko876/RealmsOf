using System;
using Riftborne.App.Spawning.Lifecycle.Abstractions;
using Riftborne.Core.Input.Commands;
using Riftborne.Core.Input.Handlers;
using Riftborne.Core.Model;

namespace Riftborne.App.Spawning.Handlers
{
    public sealed class SpawnPlayerAvatarCommandHandler : ICommandHandler<SpawnPlayerAvatarCommand>
    {
        private readonly IEntityLifecycle _lifecycle;
        private readonly GameState _state;

        public SpawnPlayerAvatarCommandHandler(IEntityLifecycle lifecycle, GameState state)
        {
            _lifecycle = lifecycle ?? throw new ArgumentNullException(nameof(lifecycle));
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public void Handle(SpawnPlayerAvatarCommand command)
        {
            GameEntityId? fixedId = command.HasFixedId ? command.FixedId : (GameEntityId?)null;

            var entityId = _lifecycle.Spawn(command.PrefabKey, command.X, command.Y, fixedId);

            _state.PlayerAvatars.Set(command.PlayerId, entityId);
        }
    }
}