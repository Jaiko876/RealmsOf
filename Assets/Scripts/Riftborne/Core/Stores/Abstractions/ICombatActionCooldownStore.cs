using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Model;

namespace Riftborne.Core.Stores.Abstractions
{
    public interface ICombatActionCooldownStore
    {
        bool CanUse(GameEntityId id, CombatActionType action, int tick);
        void Consume(GameEntityId id, CombatActionType action, int tick, int cooldownTicks);

        void Remove(GameEntityId id);
        void Clear();
    }
}