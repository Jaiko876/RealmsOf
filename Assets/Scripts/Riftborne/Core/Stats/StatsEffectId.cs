namespace Riftborne.Core.Stats
{
    // Stable ids for effects (used in save/replay/network).
    public enum StatsEffectId : int
    {
        None = 0,

        // Examples
        Berserk = 1,     // +Attack, +AttackSpeed, maybe -Defense
        Sprint = 2,      // +MoveSpeed
        StoneSkin = 3    // +Defense, -MoveSpeed
    }
}