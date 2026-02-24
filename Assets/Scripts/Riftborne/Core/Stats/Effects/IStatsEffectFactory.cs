using Riftborne.Core.Model;

namespace Riftborne.Core.Stats.Effects
{
    public interface IStatsEffectFactory
    {
        // durationTicks <= 0 => infinite (по твоей модели StatsEffect)
        StatsEffect Create(GameEntityId target, StatsEffectId id, int durationTicks, int stacks);

        StatsEffect CreateInfinite(GameEntityId target, StatsEffectId id, int stacks);
        StatsEffect CreateTimed(GameEntityId target, StatsEffectId id, int durationTicks, int stacks);
    }
}