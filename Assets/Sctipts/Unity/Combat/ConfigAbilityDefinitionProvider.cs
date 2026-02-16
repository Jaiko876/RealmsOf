using System.Collections.Generic;
using Game.Core.Combat.Abilities;
using Game.Configs;
using UnityEngine;

namespace Game.Unity.Combat
{
    public sealed class ConfigAbilityDefinitionProvider : IAbilityDefinitionProvider
    {
        private readonly Dictionary<AbilitySlot, AbilityDefinition> _map;
        private readonly DefaultAbilityDefinitionProvider _fallback = new DefaultAbilityDefinitionProvider();

        public ConfigAbilityDefinitionProvider(CombatConfigAsset asset)
        {
            _map = new Dictionary<AbilitySlot, AbilityDefinition>();

            if (asset == null)
            {
                Debug.LogError("CombatConfigAsset is null. Using default ability definitions.");
                return;
            }

            var list = asset.Abilities;
            if (list == null)
            {
                Debug.LogError("CombatConfigAsset.Abilities is null. Using default ability definitions.");
                return;
            }

            for (int i = 0; i < list.Count; i++)
            {
                var e = list[i];
                _map[e.Slot] = e.ToCore();
            }
        }

        public AbilityDefinition Get(AbilitySlot slot)
        {
            if (_map.TryGetValue(slot, out var def))
                return def;

            Debug.LogWarning("Ability definition missing in CombatConfigAsset for slot: " + slot + ". Using default.");
            return _fallback.Get(slot);
        }
    }
}
