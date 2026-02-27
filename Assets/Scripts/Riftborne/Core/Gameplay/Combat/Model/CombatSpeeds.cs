namespace Riftborne.Core.Gameplay.Combat.Model
{
    public readonly struct CombatSpeeds
    {
        public readonly float AttackSpeed;
        public readonly float ChargeSpeed;

        public CombatSpeeds(float attackSpeed, float chargeSpeed)
        {
            AttackSpeed = attackSpeed;
            ChargeSpeed = chargeSpeed;
        }

        public static CombatSpeeds Default => new CombatSpeeds(1f, 1f);
    }
}