namespace Riftborne.Core.Config
{
    public readonly struct CombatActionsTuning
    {
        public readonly PhaseWeights Light;
        public readonly PhaseWeights Heavy;

        public readonly FixedAction Parry;
        public readonly FixedAction Dodge;

        public readonly AttackMovementTuning AttackMovement;
        public readonly CancelTuning Cancel;
        
        public readonly DodgeMovementTuning DodgeMovement;

        public CombatActionsTuning(
            PhaseWeights light,
            PhaseWeights heavy,
            FixedAction parry,
            FixedAction dodge,
            AttackMovementTuning attackMovement,
            CancelTuning cancel, 
            DodgeMovementTuning dodgeMovement)
        {
            Light = light;
            Heavy = heavy;
            Parry = parry;
            Dodge = dodge;
            AttackMovement = attackMovement;
            Cancel = cancel;
            DodgeMovement = dodgeMovement;
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

        public readonly struct AttackMovementTuning
        {
            // Applied in Windup + Active only
            public readonly float LightMoveMul;
            public readonly float HeavyMoveMul;

            public AttackMovementTuning(float lightMoveMul, float heavyMoveMul)
            {
                LightMoveMul = Clamp01To1(lightMoveMul);
                HeavyMoveMul = Clamp01To1(heavyMoveMul);
            }

            private static float Clamp01To1(float v)
            {
                if (v <= 0f) return 1f;
                if (v > 1f) return 1f;
                return v;
            }
        }

        public readonly struct CancelTuning
        {
            // Light: dodge-cancel starts at Recovery begin.
            // Heavy: starts later in Recovery (0..1)
            public readonly float HeavyDodgeCancelRecoveryStart01;

            public CancelTuning(float heavyDodgeCancelRecoveryStart01)
            {
                if (heavyDodgeCancelRecoveryStart01 < 0f) heavyDodgeCancelRecoveryStart01 = 0f;
                if (heavyDodgeCancelRecoveryStart01 > 1f) heavyDodgeCancelRecoveryStart01 = 1f;
                HeavyDodgeCancelRecoveryStart01 = heavyDodgeCancelRecoveryStart01;
            }
        }
        
        public readonly struct DodgeMovementTuning
        {
            // Vx = Motor.MaxSpeedX * RollSpeedMul during Dodge.Active
            public readonly float RollSpeedMul;

            public DodgeMovementTuning(float rollSpeedMul)
            {
                // allow >1 (roll faster than run), guard nonsense
                if (rollSpeedMul < 0.1f) rollSpeedMul = 0.1f;
                if (rollSpeedMul > 3.0f) rollSpeedMul = 3.0f;
                RollSpeedMul = rollSpeedMul;
            }
        }
    }
}