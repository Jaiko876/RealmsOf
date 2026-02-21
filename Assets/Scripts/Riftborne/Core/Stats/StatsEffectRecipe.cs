namespace Riftborne.Core.Stats
{
    public readonly struct StatsEffectRecipe
    {
        public readonly EffectStacking Stacking;
        public readonly StatMod[] Mods;

        public StatsEffectRecipe(EffectStacking stacking, StatMod[] mods)
        {
            Stacking = stacking;
            Mods = mods;
        }
    }
}