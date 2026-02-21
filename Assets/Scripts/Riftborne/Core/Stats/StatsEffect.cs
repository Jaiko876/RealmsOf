using Riftborne.Core.Model;

namespace Riftborne.Core.Stats
{
    public readonly struct StatsEffect
    {
        public readonly GameEntityId Target;
        public readonly int Key; // stable id of effect type (e.g. hash / enum->int)
        public readonly EffectStacking Stacking;

        public readonly int RemainingTicks; // <= 0 means infinite
        public readonly int Stacks;         // >=1

        public readonly StatMod[] Mods;     // small array; allocate once at creation time
        public readonly int Sequence;       // assigned by store for deterministic apply order

        public StatsEffect(
            GameEntityId target,
            int key,
            EffectStacking stacking,
            int remainingTicks,
            int stacks,
            StatMod[] mods,
            int sequence)
        {
            Target = target;
            Key = key;
            Stacking = stacking;
            RemainingTicks = remainingTicks;
            Stacks = stacks < 1 ? 1 : stacks;
            Mods = mods;
            Sequence = sequence;
        }

        public bool IsInfinite => RemainingTicks <= 0;

        public StatsEffect WithRemainingTicks(int ticks)
        {
            return new StatsEffect(Target, Key, Stacking, ticks, Stacks, Mods, Sequence);
        }

        public StatsEffect WithStacks(int stacks)
        {
            return new StatsEffect(Target, Key, Stacking, RemainingTicks, stacks, Mods, Sequence);
        }
    }
}