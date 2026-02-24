using Riftborne.App.Spawning.Lifecycle.Abstractions;
using Riftborne.Core.Input.Commands;
using Riftborne.Core.Input.Handlers;

namespace Riftborne.App.Spawning.Handlers
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