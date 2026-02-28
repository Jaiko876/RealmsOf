using Riftborne.Core.Gameplay.Weapons.Model;
using Riftborne.Core.Model;

namespace Riftborne.Core.Stores.Abstractions
{
    public interface IEquippedWeaponStore
    {
        WeaponId GetOrDefault(GameEntityId id, WeaponId fallback);
        void Set(GameEntityId id, WeaponId weaponId);

        void Remove(GameEntityId id);
        void Clear();
    }
}