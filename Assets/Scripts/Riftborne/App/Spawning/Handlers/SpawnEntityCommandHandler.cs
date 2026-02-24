using Riftborne.App.Spawning.Lifecycle.Abstractions;
using Riftborne.Core.Input.Commands;
using Riftborne.Core.Input.Handlers;
using Riftborne.Core.Model;

namespace Riftborne.App.Spawning.Handlers
{
    public sealed class SpawnEntityCommandHandler : ICommandHandler<SpawnEntityCommand>
    {
        private readonly IEntityLifecycle _lifecycle;

        public SpawnEntityCommandHandler(IEntityLifecycle lifecycle)
        {
            _lifecycle = lifecycle;
        }

        public void Handle(SpawnEntityCommand command)
        {
            GameEntityId? fixedId = command.HasFixedId ? command.FixedId : (GameEntityId?)null;
            _lifecycle.Spawn(command.PrefabKey, command.X, command.Y, fixedId);
        }
    }
}