using System.Collections.Generic;
using Riftborne.Core.Model;
using Riftborne.Core.Stats;

namespace Riftborne.Core.Stores
{
    public sealed class StatsDeltaStore : IStatsDeltaStore
    {
        private readonly List<StatsDelta> _buffer = new List<StatsDelta>(256);

        public void Enqueue(GameEntityId target, StatsResource resource, int amount, StatsDeltaKind kind)
        {
            if (amount == 0) return;
            _buffer.Add(new StatsDelta(target, resource, amount, kind));
        }

        public void Heal(GameEntityId target, int hp, StatsDeltaKind kind)
        {
            if (hp <= 0) return;
            Enqueue(target, StatsResource.Hp, +hp, kind);
        }

        public void DamageHp(GameEntityId target, int hp, StatsDeltaKind kind)
        {
            if (hp <= 0) return;
            Enqueue(target, StatsResource.Hp, -hp, kind);
        }

        public void AddStamina(GameEntityId target, int amount, StatsDeltaKind kind)
        {
            if (amount <= 0) return;
            Enqueue(target, StatsResource.Stamina, +amount, kind);
        }

        public void SpendStamina(GameEntityId target, int amount, StatsDeltaKind kind)
        {
            if (amount <= 0) return;
            Enqueue(target, StatsResource.Stamina, -amount, kind);
        }

        public void AddStagger(GameEntityId target, int amount, StatsDeltaKind kind)
        {
            if (amount <= 0) return;
            Enqueue(target, StatsResource.Stagger, +amount, kind);
        }

        public void ReduceStagger(GameEntityId target, int amount, StatsDeltaKind kind)
        {
            if (amount <= 0) return;
            Enqueue(target, StatsResource.Stagger, -amount, kind);
        }

        public IReadOnlyList<StatsDelta> Drain()
        {
            return _buffer;
        }

        public void Clear()
        {
            _buffer.Clear();
        }
    }
}