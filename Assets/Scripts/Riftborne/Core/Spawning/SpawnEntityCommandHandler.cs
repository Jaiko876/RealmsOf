using Riftborne.Core.Commands;
using Riftborne.Core.Model;

namespace Riftborne.Core.Spawning
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