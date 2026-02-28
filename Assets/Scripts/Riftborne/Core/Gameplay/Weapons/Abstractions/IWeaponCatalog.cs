using Riftborne.Core.Gameplay.Weapons.Model;

namespace Riftborne.Core.Gameplay.Weapons.Abstractions
{
    public interface IWeaponCatalog
    {
        WeaponDefinition Get(WeaponId id);
        bool TryGet(WeaponId id, out WeaponDefinition def);
    }
}