using Riftborne.Core.Commands;
using Riftborne.Core.Model;

namespace Riftborne.Core.Combat.Input
{
    public readonly struct AttackPressCommand : ICommand
    {
        public int Tick { get; }
        public GameEntityId EntityId { get; }

        public AttackPressCommand(int tick, GameEntityId entityId)
        {
            Tick = tick;
            EntityId = entityId;
        }
    }
}
