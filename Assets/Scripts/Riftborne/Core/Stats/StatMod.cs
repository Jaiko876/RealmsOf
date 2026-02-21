namespace Riftborne.Core.Stats
{
    public readonly struct StatMod
    {
        public readonly StatId Stat;
        public readonly StatModKind Kind;
        public readonly float Value;

        public StatMod(StatId stat, StatModKind kind, float value)
        {
            Stat = stat;
            Kind = kind;
            Value = value;
        }
    }
}