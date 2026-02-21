using Riftborne.Core.Model;

namespace Riftborne.Core.Stats
{
    public readonly struct StatsDelta
    {
        public readonly GameEntityId Target;
        public readonly StatsResource Resource;
        public readonly int Amount; // + heal/regen, - damage/cost
        public readonly StatsDeltaKind Kind;

        public StatsDelta(GameEntityId target, StatsResource resource, int amount, StatsDeltaKind kind)
        {
            Target = target;
            Resource = resource;
            Amount = amount;
            Kind = kind;
        }
    }

    public enum StatsDeltaKind : byte
    {
        Unknown = 0,
        Regen = 1,
        Cost = 2,
        Damage = 3,
        Script = 4
    }
}