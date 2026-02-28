using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Model;

namespace Riftborne.Core.Stores.Abstractions
{
    public interface ICombatActionStore
    {
        bool TryGet(GameEntityId id, out CombatActionInstance action);
        void Set(GameEntityId id, CombatActionInstance action);
        void Remove(GameEntityId id);
        void Clear();
    }
}