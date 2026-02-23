namespace Riftborne.Core.Model.Animation
{
    public readonly struct AnimationModifiers
    {
        public readonly float AttackAnimSpeed;
        public readonly float ChargeAnimSpeed;

        public AnimationModifiers(float attackAnimSpeed, float chargeAnimSpeed)
        {
            AttackAnimSpeed = attackAnimSpeed;
            ChargeAnimSpeed = chargeAnimSpeed;
        }

        public static AnimationModifiers Default => new AnimationModifiers(1f, 1f);
    }
}