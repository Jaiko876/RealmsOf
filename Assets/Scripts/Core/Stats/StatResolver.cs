using System;
using System.Collections.Generic;
using Game.Core.Model;

namespace Game.Core.Stats
{
    /// <summary>
    /// Итоговый расчет статов по источникам с кэшированием на тик.
    /// </summary>
    public sealed class StatResolver
    {
        private readonly List<IStatSource> _sources = new List<IStatSource>(8);

        private readonly Dictionary<StatKey, float> _cache = new Dictionary<StatKey, float>(1024);
        private readonly List<StatModifier> _scratch = new List<StatModifier>(16);

        private int _currentTick = -1;

        public void SetTick(int tick)
        {
            if (tick == _currentTick)
                return;

            _currentTick = tick;
            _cache.Clear();
        }

        public void ClearSources()
        {
            _sources.Clear();
            _cache.Clear();
        }

        public void AddSource(IStatSource source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            _sources.Add(source);
            _cache.Clear();
        }

        public float Get(GameEntityId entityId, StatId statId)
        {
            var key = new StatKey(entityId, statId);

            float cached;
            if (_cache.TryGetValue(key, out cached))
                return cached;

            var value = Compute(entityId, statId);
            _cache[key] = value;
            return value;
        }

        private float Compute(GameEntityId entityId, StatId statId)
        {
            _scratch.Clear();

            // Собираем модификаторы со всех источников
            for (int i = 0; i < _sources.Count; i++)
            {
                _sources[i].TryGetModifiers(entityId, statId, _scratch);
            }

            if (_scratch.Count == 0)
                return 0f;

            // Сортируем по priority (малые раньше, большие позже)
            _scratch.Sort(CompareByPriority);

            // Применяем по порядку.
            float v = 0f;

            for (int i = 0; i < _scratch.Count; i++)
            {
                var m = _scratch[i];

                switch (m.Op)
                {
                    case StatOp.Add: v += m.Value; break;
                    case StatOp.Mul: v *= m.Value; break;
                    case StatOp.Override: v = m.Value; break;
                    case StatOp.Min: v = Math.Min(v, m.Value); break;
                    case StatOp.Max: v = Math.Max(v, m.Value); break;
                    case StatOp.ClampMin: v = Math.Max(v, m.Value); break;
                    case StatOp.ClampMax: v = Math.Min(v, m.Value); break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            return v;
        }

        private static int CompareByPriority(StatModifier a, StatModifier b)
        {
            if (a.Priority < b.Priority) return -1;
            if (a.Priority > b.Priority) return 1;
            return 0;
        }

        private readonly struct StatKey : IEquatable<StatKey>
        {
            private readonly GameEntityId _entityId;
            private readonly StatId _statId;

            public StatKey(GameEntityId entityId, StatId statId)
            {
                _entityId = entityId;
                _statId = statId;
            }

            public bool Equals(StatKey other)
            {
                return _entityId.Equals(other._entityId) && _statId == other._statId;
            }

            public override bool Equals(object obj)
            {
                return obj is StatKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 31 + _entityId.GetHashCode();
                    hash = hash * 31 + (int)_statId;
                    return hash;
                }
            }
        }
    }
}
