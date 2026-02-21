namespace Riftborne.Core.Stats
{
    public interface IStatsEffectCatalog
    {
        bool TryGetRecipe(StatsEffectId id, out StatsEffectRecipe recipe);
    }
}