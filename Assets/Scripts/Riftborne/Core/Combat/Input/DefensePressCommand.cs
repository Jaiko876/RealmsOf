using Riftborne.Core.Abstractions;
using Riftborne.Core.Model;

namespace Riftborne.Core.Combat.Input
{
    public readonly struct DefensePressCommand : ICommand
    {
        public int Tick { get; }
        public GameEntityId EntityId { get; }

        public DefensePressCommand(int tick, GameEntityId entityId)
        {
            Tick = tick;
            EntityId = entityId;
        }
    }
}
