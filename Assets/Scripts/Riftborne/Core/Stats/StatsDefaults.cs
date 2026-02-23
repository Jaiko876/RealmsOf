namespace Riftborne.Core.Stats
{
    public readonly struct StatsDefaults
    {
        public readonly int HpMax;
        public readonly int StaminaMax;
        public readonly int StaggerMax;

        public readonly float Attack;
        public readonly float Defense;
        public readonly float MoveSpeed;
        public readonly float AttackSpeed;
        public readonly float ChargeSpeed;
        public readonly float StaggerResist;

        public readonly float HpRegenPerSec;
        public readonly float StaminaRegenPerSec;
        public readonly float StaggerDecayPerSec;

        public StatsDefaults(
            int hpMax, int staminaMax, int staggerMax,
            float attack, float defense, float moveSpeed, float attackSpeed, float chargeSpeed, float staggerResist,
            float hpRegenPerSec, float staminaRegenPerSec, float staggerDecayPerSec)
        {
            HpMax = hpMax;
            StaminaMax = staminaMax;
            StaggerMax = staggerMax;

            Attack = attack;
            Defense = defense;
            MoveSpeed = moveSpeed;
            AttackSpeed = attackSpeed;
            ChargeSpeed = chargeSpeed;
            StaggerResist = staggerResist;

            HpRegenPerSec = hpRegenPerSec;
            StaminaRegenPerSec = staminaRegenPerSec;
            StaggerDecayPerSec = staggerDecayPerSec;
        }
    }
}