using System.Collections.Generic;
using Game.Core.Model;

namespace Game.Core.Stats
{
    public sealed class EntityBaseStatsSource : IStatSource
    {
        // Обычно выше дефолтов, ниже бафов/экипа.
        private const int Priority = 0;

        private readonly Dictionary<GameEntityId, Dictionary<StatId, float>> _base
            = new Dictionary<GameEntityId, Dictionary<StatId, float>>();

        public void Set(GameEntityId entityId, StatId statId, float value)
        {
            Dictionary<StatId, float> stats;
            if (!_base.TryGetValue(entityId, out stats))
            {
                stats = new Dictionary<StatId, float>();
                _base[entityId] = stats;
            }

            stats[statId] = value;
        }

        public bool TryGetModifiers(GameEntityId entityId, StatId statId, List<StatModifier> outModifiers)
        {
            Dictionary<StatId, float> stats;
            if (!_base.TryGetValue(entityId, out stats))
                return false;

            float v;
            if (!stats.TryGetValue(statId, out v))
                return false;

            outModifiers.Add(StatModifier.Add(v, Priority));
            return true;
        }
    }
}
