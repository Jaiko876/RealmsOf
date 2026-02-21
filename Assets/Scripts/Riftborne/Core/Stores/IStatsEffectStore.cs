using System.Collections.Generic;
using Riftborne.Core.Model;
using Riftborne.Core.Stats;

namespace Riftborne.Core.Stores
{
    public interface IStatsEffectStore
    {
        void AddOrStack(StatsEffect effect, int durationTicks);
        bool Remove(GameEntityId target, int key);

        // tick down durations and remove expired
        void TickDurations();

        // enumerate active effects for entity (do not allocate)
        IReadOnlyList<StatsEffect> GetEffects(GameEntityId target);

        void ClearEntity(GameEntityId target);
    }
}