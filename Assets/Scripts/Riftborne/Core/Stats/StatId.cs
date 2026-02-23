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
        ChargeSpeed = 7,
        StaggerResist = 8,

        HpRegenPerSec = 9,
        StaminaRegenPerSec = 10,
        StaggerDecayPerSec = 11,

        Count = 12
    }
}