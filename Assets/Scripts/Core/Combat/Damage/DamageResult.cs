namespace Game.Core.Combat.Damage
{
    public readonly struct DamageResult
    {
        public readonly float FinalHpDamage;
        public readonly float FinalStaminaDamage;
        public readonly float FinalStaggerBuild;

        public DamageResult(float finalHpDamage, float finalStaminaDamage, float finalStaggerBuild)
        {
            FinalHpDamage = finalHpDamage;
            FinalStaminaDamage = finalStaminaDamage;
            FinalStaggerBuild = finalStaggerBuild;
        }
    }
}
