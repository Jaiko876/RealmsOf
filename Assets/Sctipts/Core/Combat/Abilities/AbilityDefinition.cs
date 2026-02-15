namespace Game.Core.Combat.Abilities
{
    public sealed class AbilityDefinition
    {
        public AbilitySlot Slot { get; }

        public float BaseHpDamage { get; }
        public float BaseStaminaDamage { get; }
        public float BaseStaggerBuild { get; }


        public int WindupTicks { get; }
        public int ActiveTicks { get; }
        public int RecoveryTicks { get; }

        public float StaminaCost { get; }

        public bool IsAttack { get; }
        public bool IsParry { get; }
        public bool IsDodge { get; }
        public bool IsBlock { get; }

        public bool Parryable { get; }
        public bool Dodgeable { get; }

        public AbilityDefinition(
            AbilitySlot slot,
            int windupTicks,
            int activeTicks,
            int recoveryTicks,
            float staminaCost,
            bool isAttack,
            bool isParry,
            bool isDodge,
            bool isBlock,
            bool parryable,
            bool dodgeable,
            float baseHpDamage,
            float baseStaminaDamage,
            float baseStaggerBuild)
        {
            Slot = slot;
            WindupTicks = windupTicks;
            ActiveTicks = activeTicks;
            RecoveryTicks = recoveryTicks;
            StaminaCost = staminaCost;
            IsAttack = isAttack;
            IsParry = isParry;
            IsDodge = isDodge;
            IsBlock = isBlock;
            Parryable = parryable;
            Dodgeable = dodgeable;

            BaseHpDamage = baseHpDamage;
            BaseStaminaDamage = baseStaminaDamage;
            BaseStaggerBuild = baseStaggerBuild;
        }

    }
}
