using Riftborne.Core.Model;
using Riftborne.Core.Stats;

namespace Riftborne.Core.Factory
{
    public interface IStatsEffectFactory
    {
        // durationTicks <= 0 => infinite (по твоей модели StatsEffect)
        StatsEffect Create(GameEntityId target, StatsEffectId id, int durationTicks, int stacks);

        StatsEffect CreateInfinite(GameEntityId target, StatsEffectId id, int stacks);
        StatsEffect CreateTimed(GameEntityId target, StatsEffectId id, int durationTicks, int stacks);
    }
}