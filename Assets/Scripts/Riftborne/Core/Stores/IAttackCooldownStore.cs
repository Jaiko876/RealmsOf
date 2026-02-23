using Riftborne.Core.Model;

namespace Riftborne.Core.Stores
{
    public interface IAttackCooldownStore
    {
        bool CanAttack(GameEntityId id, int tick);
        void ConsumeAttack(GameEntityId id, int tick, int cooldownTicks);

        void Remove(GameEntityId id);
        void Clear();
    }
}