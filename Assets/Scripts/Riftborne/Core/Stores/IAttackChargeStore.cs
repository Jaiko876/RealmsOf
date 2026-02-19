using Riftborne.Core.Model;

namespace Riftborne.Core.Stores
{
    public interface IAttackChargeStore
    {
        void Set(GameEntityId id, bool charging, float charge01);
        bool TryGet(GameEntityId id, out bool charging, out float charge01);
        void Remove(GameEntityId id);
        void Clear();
    }
}