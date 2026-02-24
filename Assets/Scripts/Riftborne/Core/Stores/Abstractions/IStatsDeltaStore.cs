using System.Collections.Generic;
using Riftborne.Core.Model;
using Riftborne.Core.Stats;

namespace Riftborne.Core.Stores.Abstractions
{
    public interface IStatsDeltaStore
    {
        void Enqueue(GameEntityId target, StatsResource resource, int amount, StatsDeltaKind kind);

        // Удобные хелперы (чтобы не путать знаки)
        void Heal(GameEntityId target, int hp, StatsDeltaKind kind);
        void DamageHp(GameEntityId target, int hp, StatsDeltaKind kind);

        void AddStamina(GameEntityId target, int amount, StatsDeltaKind kind);
        void SpendStamina(GameEntityId target, int amount, StatsDeltaKind kind);

        void AddStagger(GameEntityId target, int amount, StatsDeltaKind kind);
        void ReduceStagger(GameEntityId target, int amount, StatsDeltaKind kind);

        IReadOnlyList<StatsDelta> Drain();
        void Clear();
    }
}