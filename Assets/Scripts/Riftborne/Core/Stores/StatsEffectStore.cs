using System.Collections.Generic;
using Riftborne.Core.Model;
using Riftborne.Core.Stats;

namespace Riftborne.Core.Stores
{
    public sealed class StatsEffectStore : IStatsEffectStore
    {
        private readonly Dictionary<GameEntityId, List<StatsEffect>> _map = new Dictionary<GameEntityId, List<StatsEffect>>();
        private readonly List<StatsEffect> _empty = new List<StatsEffect>(0);

        private int _seq;

        public void AddOrStack(StatsEffect effect, int durationTicks)
        {
            if (!_map.TryGetValue(effect.Target, out var list))
            {
                list = new List<StatsEffect>(8);
                _map[effect.Target] = list;
            }

            // assign deterministic sequence for new effects
            if (effect.Sequence == 0)
            {
                _seq++;
                effect = new StatsEffect(
                    effect.Target, effect.Key, effect.Stacking,
                    remainingTicks: durationTicks,
                    stacks: effect.Stacks,
                    mods: effect.Mods,
                    sequence: _seq);
            }
            else
            {
                effect = effect.WithRemainingTicks(durationTicks);
            }

            int idx = FindByKey(list, effect.Key);
            if (idx < 0)
            {
                list.Add(effect);
                return;
            }

            var old = list[idx];
            switch (effect.Stacking)
            {
                case EffectStacking.Replace:
                    list[idx] = effect;
                    break;

                case EffectStacking.Refresh:
                    // keep old mods/strength, only refresh duration
                    list[idx] = old.WithRemainingTicks(durationTicks);
                    break;

                case EffectStacking.AddStacks:
                {
                    int newStacks = old.Stacks + effect.Stacks;
                    // refresh duration as well (common roguelite behavior)
                    list[idx] = old.WithStacks(newStacks).WithRemainingTicks(durationTicks);
                    break;
                }
            }
        }

        public bool Remove(GameEntityId target, int key)
        {
            if (!_map.TryGetValue(target, out var list))
                return false;

            int idx = FindByKey(list, key);
            if (idx < 0)
                return false;

            int last = list.Count - 1;
            list[idx] = list[last];
            list.RemoveAt(last);
            return true;
        }

        public void TickDurations()
        {
            foreach (var kv in _map)
            {
                var list = kv.Value;

                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var e = list[i];

                    if (e.IsInfinite)
                        continue;

                    int t = e.RemainingTicks - 1;
                    if (t <= 0)
                    {
                        int last = list.Count - 1;
                        list[i] = list[last];
                        list.RemoveAt(last);
                        continue;
                    }

                    list[i] = e.WithRemainingTicks(t);
                }
            }
        }

        public IReadOnlyList<StatsEffect> GetEffects(GameEntityId target)
        {
            if (_map.TryGetValue(target, out var list))
                return list;
            return _empty;
        }

        public void ClearEntity(GameEntityId target)
        {
            _map.Remove(target);
        }

        private static int FindByKey(List<StatsEffect> list, int key)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Key == key)
                    return i;
            }
            return -1;
        }
    }
}