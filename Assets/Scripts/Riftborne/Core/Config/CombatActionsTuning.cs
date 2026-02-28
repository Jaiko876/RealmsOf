namespace Riftborne.Core.Config
{
    public readonly struct CombatActionsTuning
    {
        public readonly PhaseWeights Light;
        public readonly PhaseWeights Heavy;

        public readonly FixedAction Parry;
        public readonly FixedAction Dodge;

        public CombatActionsTuning(
            PhaseWeights light,
            PhaseWeights heavy,
            FixedAction parry,
            FixedAction dodge)
        {
            Light = light;
            Heavy = heavy;
            Parry = parry;
            Dodge = dodge;
        }

        public readonly struct PhaseWeights
        {
            public readonly int WindupWeight;
            public readonly int ActiveWeight;
            public readonly int RecoveryWeight;

            public PhaseWeights(int windupWeight, int activeWeight, int recoveryWeight)
            {
                WindupWeight = windupWeight < 0 ? 0 : windupWeight;
                ActiveWeight = activeWeight < 0 ? 0 : activeWeight;
                RecoveryWeight = recoveryWeight < 0 ? 0 : recoveryWeight;
            }
        }

        public readonly struct FixedAction
        {
            public readonly int DurationBaseTicks;
            public readonly int CooldownBaseTicks;

            public readonly PhaseWeights Phases;

            public FixedAction(int durationBaseTicks, int cooldownBaseTicks, PhaseWeights phases)
            {
                DurationBaseTicks = durationBaseTicks < 0 ? 0 : durationBaseTicks;
                CooldownBaseTicks = cooldownBaseTicks < 0 ? 0 : cooldownBaseTicks;
                Phases = phases;
            }
        }
    }
}