using System;
using System.Collections.Generic;
using Riftborne.Core.Gameplay.Weapons.Abstractions;
using Riftborne.Core.Gameplay.Weapons.Model;

namespace Riftborne.App.Weapons.Catalog
{
    public sealed class WeaponCatalog : IWeaponCatalog
    {
        private readonly Dictionary<WeaponId, WeaponDefinition> _map;

        public WeaponCatalog(WeaponDefinition[] defs)
        {
            _map = new Dictionary<WeaponId, WeaponDefinition>(defs != null ? defs.Length : 4);

            if (defs == null) return;

            for (int i = 0; i < defs.Length; i++)
            {
                var d = defs[i];
                _map[d.Id] = d;
            }
        }

        public WeaponDefinition Get(WeaponId id)
        {
            if (!_map.TryGetValue(id, out var d))
                throw new InvalidOperationException("Unknown WeaponId: " + id);
            return d;
        }

        public bool TryGet(WeaponId id, out WeaponDefinition def) => _map.TryGetValue(id, out def);
    }
}