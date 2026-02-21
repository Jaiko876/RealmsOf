namespace Riftborne.Core.Stats
{
    public enum StatId : byte
    {
        HpMax = 0,
        StaminaMax = 1,
        StaggerMax = 2,

        Attack = 3,
        Defense = 4,
        MoveSpeed = 5,
        AttackSpeed = 6,
        StaggerResist = 7,

        HpRegenPerSec = 8,
        StaminaRegenPerSec = 9,
        StaggerDecayPerSec = 10,

        Count = 11
    }
}