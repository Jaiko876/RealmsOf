using Game.Core.Model;
using Game.Core.Abstractions;

namespace Game.Core.Combat.Abilities
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
