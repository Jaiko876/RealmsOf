using System.Collections.Generic;
using Game.Core.Combat.Abilities;
using Game.Configs;

namespace Game.Unity.Combat
{
    public sealed class ConfigAbilityDefinitionProvider : IAbilityDefinitionProvider
    {
        private readonly Dictionary<AbilitySlot, AbilityDefinition> _map;

        public ConfigAbilityDefinitionProvider(CombatConfigAsset asset)
        {
            _map = new Dictionary<AbilitySlot, AbilityDefinition>();

            var list = asset.Abilities;
            for (int i = 0; i < list.Count; i++)
            {
                var e = list[i];
                _map[e.Slot] = e.ToCore();
            }
        }

        public AbilityDefinition Get(AbilitySlot slot)
        {
            return _map[slot];
        }
    }
}
