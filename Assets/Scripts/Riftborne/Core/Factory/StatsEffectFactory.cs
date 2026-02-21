using System;
using Riftborne.Core.Model;
using Riftborne.Core.Stats;

namespace Riftborne.Core.Factory
{
    public sealed class StatsEffectFactory : IStatsEffectFactory
    {
        private readonly IStatsEffectCatalog _catalog;

        public StatsEffectFactory(IStatsEffectCatalog catalog)
        {
            _catalog = catalog;
        }

        public StatsEffect Create(GameEntityId target, StatsEffectId id, int durationTicks, int stacks)
        {
            if (durationTicks <= 0)
                return CreateInfinite(target, id, stacks);

            return CreateTimed(target, id, durationTicks, stacks);
        }

        public StatsEffect CreateInfinite(GameEntityId target, StatsEffectId id, int stacks)
        {
            if (!_catalog.TryGetRecipe(id, out var recipe))
                throw new InvalidOperationException("Unknown stats effect id: " + id);

            return new StatsEffect(
                target: target,
                key: (int)id,
                stacking: recipe.Stacking,
                remainingTicks: 0,           // <=0 == infinite
                stacks: stacks,
                mods: recipe.Mods,
                sequence: 0                  // will be assigned by store
            );
        }

        public StatsEffect CreateTimed(GameEntityId target, StatsEffectId id, int durationTicks, int stacks)
        {
            if (durationTicks <= 0)
                throw new ArgumentOutOfRangeException(nameof(durationTicks), "durationTicks must be > 0 for timed effects.");

            if (!_catalog.TryGetRecipe(id, out var recipe))
                throw new InvalidOperationException("Unknown stats effect id: " + id);

            return new StatsEffect(
                target: target,
                key: (int)id,
                stacking: recipe.Stacking,
                remainingTicks: durationTicks,
                stacks: stacks,
                mods: recipe.Mods,
                sequence: 0
            );
        }
    }
}