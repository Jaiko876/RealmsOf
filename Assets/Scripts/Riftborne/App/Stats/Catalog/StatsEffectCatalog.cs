using System.Collections.Generic;
using Riftborne.Core.Stats;

namespace Riftborne.App.Stats.Catalog
{
    public sealed class StatsEffectCatalog : IStatsEffectCatalog
    {
        private readonly Dictionary<StatsEffectId, StatsEffectRecipe> _recipes =
            new Dictionary<StatsEffectId, StatsEffectRecipe>();

        public StatsEffectCatalog()
        {
            // Register default recipes here (or inject builder later).
            // Mods arrays are allocated ONCE and reused.

            _recipes[StatsEffectId.Berserk] = new StatsEffectRecipe(
                stacking: EffectStacking.Replace,
                mods: new[]
                {
                    new StatMod(StatId.Attack, StatModKind.Add, 10f),
                    new StatMod(StatId.AttackSpeed, StatModKind.Mul, 1.25f),
                    new StatMod(StatId.Defense, StatModKind.Mul, 0.85f),
                });

            _recipes[StatsEffectId.Sprint] = new StatsEffectRecipe(
                stacking: EffectStacking.Refresh,
                mods: new[]
                {
                    new StatMod(StatId.MoveSpeed, StatModKind.Mul, 1.35f),
                });

            _recipes[StatsEffectId.StoneSkin] = new StatsEffectRecipe(
                stacking: EffectStacking.Replace,
                mods: new[]
                {
                    new StatMod(StatId.Defense, StatModKind.Add, 15f),
                    new StatMod(StatId.MoveSpeed, StatModKind.Mul, 0.90f),
                });
        }

        public bool TryGetRecipe(StatsEffectId id, out StatsEffectRecipe recipe)
        {
            return _recipes.TryGetValue(id, out recipe);
        }
    }
}