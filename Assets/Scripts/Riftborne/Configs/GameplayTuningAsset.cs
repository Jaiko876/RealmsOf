using System;
using Riftborne.Core.Config;
using UnityEngine;
using UnityEngine.Serialization;

namespace Riftborne.Configs
{
    [CreateAssetMenu(menuName = "Riftborne/Config/GameplayTuning", fileName = "GameplayTuning")]
    public sealed class GameplayTuningAsset : ScriptableObject, IGameplayTuning
    {
        [Header("Combat Input")] [SerializeField]
        private CombatInputSection _combatInput = CombatInputSection.Default;

        [Header("Stats -> Physics")] [SerializeField]
        private StatsToPhysicsSection _statsToPhysics = StatsToPhysicsSection.Default;

        [Header("Input")] [SerializeField] private InputSection _input = InputSection.Default;

        [Header("Physics Probes")] [SerializeField]
        private PhysicsProbesSection _physicsProbes = PhysicsProbesSection.Default;

        [Header("Physics World")] [SerializeField]
        private PhysicsWorldSection _physicsWorld = PhysicsWorldSection.Default;

        [Header("Combat Animation (Authoritative durations)")] [SerializeField]
        private CombatAnimationSection _combatAnimation = CombatAnimationSection.Default;

        [Header("Combat Actions (Phases & Parry/Dodge)")] [SerializeField]
        private CombatActionsSection _combatActions = CombatActionsSection.Default;

        [Header("Combat Hit (Layers)")] [SerializeField]
        private CombatHitSection _combatHit = CombatHitSection.Default;

        [Header("Combat Damage (Rules numbers)")] [SerializeField]
        private CombatDamageSection _combatDamage = CombatDamageSection.Default;
        
        [Header("Defence Input")] [SerializeField]
        private DefenceInputSection _defenceInput = DefenceInputSection.Default;


        public DefenseInputTuning DefenseInput
            => new DefenseInputTuning(
                _defenceInput.ParryMaxTapTicks,
                _defenceInput.BlockStartTicks
                );
        

        public CombatActionsTuning CombatActions
            => new CombatActionsTuning(
                new CombatActionsTuning.PhaseWeights(
                    _combatActions.LightWindupWeight,
                    _combatActions.LightActiveWeight,
                    _combatActions.LightRecoveryWeight),
                new CombatActionsTuning.PhaseWeights(
                    _combatActions.HeavyWindupWeight,
                    _combatActions.HeavyActiveWeight,
                    _combatActions.HeavyRecoveryWeight),
                new CombatActionsTuning.FixedAction(
                    _combatActions.ParryDurationBaseTicks,
                    _combatActions.ParryCooldownBaseTicks,
                    new CombatActionsTuning.PhaseWeights(
                        _combatActions.ParryWindupWeight,
                        _combatActions.ParryActiveWeight,
                        _combatActions.ParryRecoveryWeight)),
                new CombatActionsTuning.FixedAction(
                    _combatActions.DodgeDurationBaseTicks,
                    _combatActions.DodgeCooldownBaseTicks,
                    new CombatActionsTuning.PhaseWeights(
                        _combatActions.DodgeWindupWeight,
                        _combatActions.DodgeActiveWeight,
                        _combatActions.DodgeRecoveryWeight)),
                new CombatActionsTuning.FixedAction( // NEW DodgeDash
                    _combatActions.DodgeDashDurationBaseTicks,
                    _combatActions.DodgeDashCooldownBaseTicks,
                    new CombatActionsTuning.PhaseWeights(
                        _combatActions.DodgeDashWindupWeight,
                        _combatActions.DodgeDashActiveWeight,
                        _combatActions.DodgeDashRecoveryWeight)),
                new CombatActionsTuning.AttackMovementTuning(
                    _combatActions.LightAttackMoveMul,
                    _combatActions.HeavyAttackMoveMul),
                new CombatActionsTuning.CancelTuning(
                    _combatActions.HeavyDodgeCancelRecoveryStart01),
                new CombatActionsTuning.DodgeMovementTuning(
                    _combatActions.DodgeRollSpeedMul,
                    _combatActions.DodgeDashSpeedMul),
                new CombatActionsTuning.PerfectDodgeTuning(
                    _combatActions.PerfectDodgeWindowTicks)
            );

        public CombatHitTuning CombatHit
            => new CombatHitTuning(_combatHit.TargetLayers.value);

        public CombatDamageTuning CombatDamage
            => new CombatDamageTuning(
                _combatDamage.LightHpMul,
                _combatDamage.LightStaminaDamage,
                _combatDamage.LightStagger,
                _combatDamage.HeavyHpMul,
                _combatDamage.HeavyStaminaDamage,
                _combatDamage.HeavyStagger,
                _combatDamage.ParrySuccessAttackerStagger,
                _combatDamage.DodgeSuccessAttackerStaminaDamage,
                _combatDamage.DodgeSuccessAttackerStagger,
                _combatDamage.ParryFailDefenderStaminaDamage,
                _combatDamage.DodgeFailExtraDefenderStagger,
                _combatDamage.BlockLightStaminaDamage,
                _combatDamage.BlockLightStaggerBuild,
                _combatDamage.BlockHeavyStaminaDamage,
                _combatDamage.BlockHeavyStaggerBuild,
                _combatDamage.BlockHeavyHpMul
            );

        [Serializable]
        private struct CombatHitSection
        {
            public LayerMask TargetLayers;

            public static CombatHitSection Default => new CombatHitSection
            {
                TargetLayers = -1 // Everything, потом выставишь аккуратно (Players+Enemies)
            };
        }

        [Serializable]
        private struct CombatActionsSection
        {
            // phase weights for splitting total attack duration
            public int LightWindupWeight;
            public int LightActiveWeight;
            public int LightRecoveryWeight;

            public int HeavyWindupWeight;
            public int HeavyActiveWeight;
            public int HeavyRecoveryWeight;

            // Parry fixed action
            public int ParryDurationBaseTicks;
            public int ParryCooldownBaseTicks;
            public int ParryWindupWeight;
            public int ParryActiveWeight;
            public int ParryRecoveryWeight;

            // Dodge fixed action
            public int DodgeDurationBaseTicks;
            public int DodgeCooldownBaseTicks;
            public int DodgeWindupWeight;
            public int DodgeActiveWeight;
            public int DodgeRecoveryWeight;

            public float LightAttackMoveMul;
            public float HeavyAttackMoveMul;

            public float HeavyDodgeCancelRecoveryStart01;
            
            public int PerfectDodgeWindowTicks;

            public int DodgeDashDurationBaseTicks;
            public int DodgeDashCooldownBaseTicks;
            public int DodgeDashWindupWeight;
            public int DodgeDashActiveWeight;
            public int DodgeDashRecoveryWeight;

            public float DodgeRollSpeedMul;
            public float DodgeDashSpeedMul;

            public static CombatActionsSection Default => new CombatActionsSection
            {
                LightWindupWeight = 4,
                LightActiveWeight = 6,
                LightRecoveryWeight = 6,

                HeavyWindupWeight = 6,
                HeavyActiveWeight = 8,
                HeavyRecoveryWeight = 8,

                ParryDurationBaseTicks = 14,
                ParryCooldownBaseTicks = 18,
                ParryWindupWeight = 2,
                ParryActiveWeight = 6,
                ParryRecoveryWeight = 6,

                DodgeDurationBaseTicks = 16,
                DodgeCooldownBaseTicks = 20,
                DodgeWindupWeight = 0,
                DodgeActiveWeight = 8,
                DodgeRecoveryWeight = 8,

                LightAttackMoveMul = 0.65f,
                HeavyAttackMoveMul = 0.55f,
                HeavyDodgeCancelRecoveryStart01 = 0.75f,
                PerfectDodgeWindowTicks = 3,

                DodgeDashDurationBaseTicks = 10,
                DodgeDashCooldownBaseTicks = 0,
                DodgeDashWindupWeight = 0,
                DodgeDashActiveWeight = 4,
                DodgeDashRecoveryWeight = 6,

                DodgeRollSpeedMul = 1.25f,
                DodgeDashSpeedMul = 1.90f
            };
        }

        [Serializable]
        private struct CombatDamageSection
        {
            public float LightHpMul;
            public int LightStaminaDamage;
            public int LightStagger;

            public float HeavyHpMul;
            public int HeavyStaminaDamage;
            public int HeavyStagger;

            public int ParrySuccessAttackerStagger;
            public int DodgeSuccessAttackerStaminaDamage;
            public int DodgeSuccessAttackerStagger;

            public int ParryFailDefenderStaminaDamage;
            public int DodgeFailExtraDefenderStagger;
            
            public int BlockLightStaminaDamage;
            public int BlockLightStaggerBuild;
            public int BlockHeavyStaminaDamage;
            public int BlockHeavyStaggerBuild;
            public float BlockHeavyHpMul;

            public static CombatDamageSection Default => new CombatDamageSection
            {
                LightHpMul = 1.0f,
                LightStaminaDamage = 6,
                LightStagger = 10,

                HeavyHpMul = 1.6f,
                HeavyStaminaDamage = 12,
                HeavyStagger = 20,

                ParrySuccessAttackerStagger = 18,
                DodgeSuccessAttackerStaminaDamage = 25,
                DodgeSuccessAttackerStagger = 8,

                ParryFailDefenderStaminaDamage = 12,
                DodgeFailExtraDefenderStagger = 12,
                
                BlockLightStaminaDamage = 8,
                BlockLightStaggerBuild = 6,

                BlockHeavyStaminaDamage = 22,
                BlockHeavyStaggerBuild = 28,
                BlockHeavyHpMul = 0.25f
            };
        }

        public CombatAnimationTuning CombatAnimation
            => new CombatAnimationTuning(
                _combatAnimation.LightAttackDurationBaseTicks,
                _combatAnimation.HeavyAttackDurationBaseTicks);

        public CombatInputTuning CombatInput
            => new CombatInputTuning(
                _combatInput.HeavyThresholdBaseTicks,
                _combatInput.FullChargeExtraBaseTicks,
                _combatInput.AttackCooldownBaseTicks,
                _combatInput.MinAttackSpeed,
                _combatInput.MaxAttackSpeed,
                _combatInput.MinChargeSpeed,
                _combatInput.MaxChargeSpeed);

        public StatsToPhysicsTuning StatsToPhysics
            => new StatsToPhysicsTuning(_statsToPhysics.MinMoveSpeedMultiplier, _statsToPhysics.MaxMoveSpeedMultiplier);

        public InputTuning Input
            => new InputTuning(_input.FacingDeadzone, _input.MoveSpeedDeadzone01);

        public PhysicsProbesTuning PhysicsProbes
            => new PhysicsProbesTuning(
                new PhysicsProbesTuning.GroundProbeTuning(
                    _physicsProbes.GroundSkin,
                    _physicsProbes.GroundCheckDepth,
                    _physicsProbes.GroundWidthMultiplier,
                    _physicsProbes.GroundProbeHeight,
                    _physicsProbes.GroundLayers.value),
                new PhysicsProbesTuning.WallProbeTuning(
                    _physicsProbes.WallSkin,
                    _physicsProbes.WallCheckDistance,
                    _physicsProbes.WallProbeThickness,
                    _physicsProbes.WallHeightShrink,
                    _physicsProbes.WallMinWallNormalAbsX,
                    _physicsProbes.WallLayers.value));

        public PhysicsWorldTuning PhysicsWorld
            => new PhysicsWorldTuning(_physicsWorld.MaxSubSteps);
        
        [Serializable]
        private struct CombatInputSection
        {
            public int HeavyThresholdBaseTicks;
            public int FullChargeExtraBaseTicks;

            [FormerlySerializedAs("LightCooldownBaseTicks")]
            public int AttackCooldownBaseTicks;

            public float MinAttackSpeed;
            public float MaxAttackSpeed;

            public float MinChargeSpeed;
            public float MaxChargeSpeed;

            public static CombatInputSection Default => new CombatInputSection
            {
                HeavyThresholdBaseTicks = 18,
                FullChargeExtraBaseTicks = 42,
                AttackCooldownBaseTicks = 18,
                MinAttackSpeed = 0.20f,
                MaxAttackSpeed = 3.00f,
                MinChargeSpeed = 0.20f,
                MaxChargeSpeed = 3.00f
            };
        }

        [Serializable]
        private struct CombatAnimationSection
        {
            public int LightAttackDurationBaseTicks;
            public int HeavyAttackDurationBaseTicks;

            public static CombatAnimationSection Default => new CombatAnimationSection
            {
                LightAttackDurationBaseTicks = 16,
                HeavyAttackDurationBaseTicks = 22
            };
        }

        [Serializable]
        private struct StatsToPhysicsSection
        {
            public float MinMoveSpeedMultiplier;
            public float MaxMoveSpeedMultiplier;

            public static StatsToPhysicsSection Default => new StatsToPhysicsSection
            {
                MinMoveSpeedMultiplier = 0.10f,
                MaxMoveSpeedMultiplier = 3.00f
            };
        }

        [Serializable]
        private struct InputSection
        {
            public float FacingDeadzone;
            public float MoveSpeedDeadzone01;

            public static InputSection Default => new InputSection
            {
                FacingDeadzone = 0.10f,
                MoveSpeedDeadzone01 = 0.01f
            };
        }

        [Serializable]
        private struct PhysicsProbesSection
        {
            [Header("Layer Masks (multi-select)")] public LayerMask GroundLayers; // несколько слоёв для grounded-check
            public LayerMask WallLayers; // несколько слоёв для wall-check

            public float GroundSkin;
            public float GroundCheckDepth;
            public float GroundWidthMultiplier;
            public float GroundProbeHeight;

            public float WallSkin;
            public float WallCheckDistance;
            public float WallProbeThickness;
            public float WallHeightShrink;
            public float WallMinWallNormalAbsX;

            public static PhysicsProbesSection Default => new PhysicsProbesSection
            {
                GroundLayers = -1, // Everything по умолчанию, потом в инспекторе выставишь нормально
                WallLayers = -1,

                GroundSkin = 0.02f,
                GroundCheckDepth = 0.08f,
                GroundWidthMultiplier = 0.90f,
                GroundProbeHeight = 0.02f,

                WallSkin = 0.02f,
                WallCheckDistance = 0.08f,
                WallProbeThickness = 0.02f,
                WallHeightShrink = 0.12f,
                WallMinWallNormalAbsX = 0.60f
            };
        }

        [Serializable]
        private struct PhysicsWorldSection
        {
            public int MaxSubSteps;

            public static PhysicsWorldSection Default => new PhysicsWorldSection
            {
                MaxSubSteps = 8
            };
        }
        
        [Serializable]
        private struct DefenceInputSection
        {
            public int ParryMaxTapTicks;
            public int BlockStartTicks;

            public static DefenceInputSection Default => new DefenceInputSection
            {
                ParryMaxTapTicks = 8,
                BlockStartTicks = 10
            };
        }
    }
}