using Riftborne.Core.Commands;

namespace Riftborne.Core.Spawning
{
    public sealed class DespawnEntityCommandHandler : ICommandHandler<DespawnEntityCommand>
    {
        private readonly IEntityLifecycle _lifecycle;

        public DespawnEntityCommandHandler(IEntityLifecycle lifecycle)
        {
            _lifecycle = lifecycle;
        }

        public void Handle(DespawnEntityCommand command)
        {
            _lifecycle.Despawn(command.EntityId);
        }
    }
}