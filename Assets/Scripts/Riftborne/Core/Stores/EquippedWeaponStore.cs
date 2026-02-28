using System.Collections.Generic;
using Riftborne.Core.Gameplay.Weapons.Model;
using Riftborne.Core.Model;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.Core.Stores
{
    public sealed class EquippedWeaponStore : IEquippedWeaponStore
    {
        private readonly Dictionary<GameEntityId, WeaponId> _map = new Dictionary<GameEntityId, WeaponId>(128);

        public WeaponId GetOrDefault(GameEntityId id, WeaponId fallback)
        {
            return _map.TryGetValue(id, out var w) ? w : fallback;
        }

        public void Set(GameEntityId id, WeaponId weaponId)
        {
            _map[id] = weaponId;
        }

        public void Remove(GameEntityId id) => _map.Remove(id);
        public void Clear() => _map.Clear();
    }
}