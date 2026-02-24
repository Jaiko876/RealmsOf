using Riftborne.Core.Input.Commands.Abstractions;
using Riftborne.Core.Model;

namespace Riftborne.Core.Input.Commands
{
    public readonly struct DespawnEntityCommand : ICommand
    {
        public int Tick { get; }
        public GameEntityId EntityId { get; }

        public DespawnEntityCommand(int tick, GameEntityId entityId)
        {
            Tick = tick;
            EntityId = entityId;
        }
    }
}