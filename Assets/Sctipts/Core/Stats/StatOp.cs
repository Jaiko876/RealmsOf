namespace Game.Core.Stats
{
    public enum StatOp
    {
        Add,        // value += x
        Mul,        // value *= x
        Override,   // value = x
        Min,        // value = min(value, x)
        Max,        // value = max(value, x)
        ClampMin,   // value = max(value, x)
        ClampMax    // value = min(value, x)
    }

    public readonly struct StatModifier
    {
        public readonly StatOp Op;
        public readonly float Value;
        public readonly int Priority;

        public StatModifier(StatOp op, float value, int priority)
        {
            Op = op;
            Value = value;
            Priority = priority;
        }

        public static StatModifier Add(float value, int priority) => new StatModifier(StatOp.Add, value, priority);
        public static StatModifier Mul(float value, int priority) => new StatModifier(StatOp.Mul, value, priority);
        public static StatModifier Override(float value, int priority) => new StatModifier(StatOp.Override, value, priority);
        public static StatModifier Min(float value, int priority) => new StatModifier(StatOp.Min, value, priority);
        public static StatModifier Max(float value, int priority) => new StatModifier(StatOp.Max, value, priority);
        public static StatModifier ClampMin(float value, int priority) => new StatModifier(StatOp.ClampMin, value, priority);
        public static StatModifier ClampMax(float value, int priority) => new StatModifier(StatOp.ClampMax, value, priority);
    }
}
