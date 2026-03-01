namespace Riftborne.Core.Config
{
    public readonly struct CombatDamageTuning
    {
        public readonly float LightHpMul;
        public readonly int LightStaminaDamage;
        public readonly int LightStagger;

        public readonly float HeavyHpMul;
        public readonly int HeavyStaminaDamage;
        public readonly int HeavyStagger;

        public readonly int BlockLightStaminaDamage;
        public readonly int BlockLightStaggerBuild;
        public readonly int BlockHeavyStaminaDamage;
        public readonly int BlockHeavyStaggerBuild;
        public readonly float BlockHeavyHpMul;

        // Defender reactions
        public readonly int ParrySuccessAttackerStagger;     // parry light => attacker stagger
        public readonly int DodgeSuccessAttackerStaminaDamage; // dodge heavy => attacker stamina loss
        public readonly int DodgeSuccessAttackerStagger;     // + micro stagger

        public readonly int ParryFailDefenderStaminaDamage;  // parry heavy fail => defender stamina loss
        public readonly int DodgeFailExtraDefenderStagger;   // dodge light fail => extra stagger

        public CombatDamageTuning(
            float lightHpMul,
            int lightStaminaDamage,
            int lightStagger,
            float heavyHpMul,
            int heavyStaminaDamage,
            int heavyStagger,
            int parrySuccessAttackerStagger,
            int dodgeSuccessAttackerStaminaDamage,
            int dodgeSuccessAttackerStagger,
            int parryFailDefenderStaminaDamage,
            int dodgeFailExtraDefenderStagger, 
            int blockLightStaminaDamage, 
            int blockLightStaggerBuild, 
            int blockHeavyStaminaDamage, 
            int blockHeavyStaggerBuild, 
            float blockHeavyHpMul)
        {
            LightHpMul = lightHpMul;
            LightStaminaDamage = lightStaminaDamage;
            LightStagger = lightStagger;

            HeavyHpMul = heavyHpMul;
            HeavyStaminaDamage = heavyStaminaDamage;
            HeavyStagger = heavyStagger;

            ParrySuccessAttackerStagger = parrySuccessAttackerStagger;
            DodgeSuccessAttackerStaminaDamage = dodgeSuccessAttackerStaminaDamage;
            DodgeSuccessAttackerStagger = dodgeSuccessAttackerStagger;

            ParryFailDefenderStaminaDamage = parryFailDefenderStaminaDamage;
            DodgeFailExtraDefenderStagger = dodgeFailExtraDefenderStagger;
            BlockLightStaminaDamage = blockLightStaminaDamage;
            BlockLightStaggerBuild = blockLightStaggerBuild;
            BlockHeavyStaminaDamage = blockHeavyStaminaDamage;
            BlockHeavyStaggerBuild = blockHeavyStaggerBuild;
            BlockHeavyHpMul = blockHeavyHpMul;
        }
    }
}