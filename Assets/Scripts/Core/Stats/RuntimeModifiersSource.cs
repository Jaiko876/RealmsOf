using System.Collections.Generic;
using Game.Core.Model;

namespace Game.Core.Stats
{
    public sealed class RuntimeModifiersSource : IStatSource
    {
        // Обычно выше базы
        private const int PriorityBase = 100;

        private readonly Dictionary<GameEntityId, List<RuntimeMod>> _mods
            = new Dictionary<GameEntityId, List<RuntimeMod>>();

        public void Add(GameEntityId entityId, StatId statId, StatOp op, float value, int extraPriority)
        {
            List<RuntimeMod> list;
            if (!_mods.TryGetValue(entityId, out list))
            {
                list = new List<RuntimeMod>(8);
                _mods[entityId] = list;
            }

            list.Add(new RuntimeMod(statId, new StatModifier(op, value, PriorityBase + extraPriority)));
        }

        public void Clear(GameEntityId entityId)
        {
            _mods.Remove(entityId);
        }

        public bool TryGetModifiers(GameEntityId entityId, StatId statId, List<StatModifier> outModifiers)
        {
            List<RuntimeMod> list;
            if (!_mods.TryGetValue(entityId, out list))
                return false;

            bool any = false;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].StatId != statId)
                    continue;

                outModifiers.Add(list[i].Modifier);
                any = true;
            }

            return any;
        }

        private readonly struct RuntimeMod
        {
            public readonly StatId StatId;
            public readonly StatModifier Modifier;

            public RuntimeMod(StatId statId, StatModifier modifier)
            {
                StatId = statId;
                Modifier = modifier;
            }
        }
    }
}
