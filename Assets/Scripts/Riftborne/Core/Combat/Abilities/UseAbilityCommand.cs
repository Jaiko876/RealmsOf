using Riftborne.Core.Commands;
using Riftborne.Core.Model;

namespace Riftborne.Core.Combat.Abilities
{
    public sealed class UseAbilityCommand : ICommand
    {
        public int Tick { get; }
        public GameEntityId EntityId { get; }
        public AbilitySlot Slot { get; }

        public UseAbilityCommand(int tick, GameEntityId entityId, AbilitySlot slot)
        {
            Tick = tick;
            EntityId = entityId;
            Slot = slot;
        }
    }
}
