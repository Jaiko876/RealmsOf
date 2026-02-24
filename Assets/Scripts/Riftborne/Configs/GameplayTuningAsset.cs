using UnityEngine;
using Riftborne.Core.Config;

namespace Riftborne.Configs
{
    [CreateAssetMenu(menuName = "Riftborne/Config/GameplayTuning", fileName = "GameplayTuning")]
    public sealed class GameplayTuningAsset : ScriptableObject, IGameplayTuning
    {
        [Header("Combat Input")]
        [SerializeField] private CombatInputSection _combatInput = CombatInputSection.Default;

        [Header("Stats -> Physics")]
        [SerializeField] private StatsToPhysicsSection _statsToPhysics = StatsToPhysicsSection.Default;

        [Header("Input")]
        [SerializeField] private InputSection _input = InputSection.Default;

        [Header("Physics Probes")]
        [SerializeField] private PhysicsProbesSection _physicsProbes = PhysicsProbesSection.Default;

        [Header("Physics World")]
        [SerializeField] private PhysicsWorldSection _physicsWorld = PhysicsWorldSection.Default;
        
        [Header("Combat Animation (Authoritative durations)")]
        [SerializeField] private CombatAnimationSection _combatAnimation = CombatAnimationSection.Default;
        
        public CombatAnimationTuning CombatAnimation
            => new CombatAnimationTuning(
                _combatAnimation.LightAttackDurationBaseTicks,
                _combatAnimation.HeavyAttackDurationBaseTicks,
                _combatAnimation.MinAnimatorSpeed,
                _combatAnimation.MaxAnimatorSpeed);

        public CombatInputTuning CombatInput
            => new CombatInputTuning(
                _combatInput.HeavyThresholdBaseTicks,
                _combatInput.FullChargeExtraBaseTicks,
                _combatInput.LightCooldownBaseTicks,
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

        [System.Serializable]
        private struct CombatInputSection
        {
            public int HeavyThresholdBaseTicks;
            public int FullChargeExtraBaseTicks;

            public int LightCooldownBaseTicks;

            public float MinAttackSpeed;
            public float MaxAttackSpeed;

            public float MinChargeSpeed;
            public float MaxChargeSpeed;

            public static CombatInputSection Default => new CombatInputSection
            {
                HeavyThresholdBaseTicks = 18,
                FullChargeExtraBaseTicks = 42,
                LightCooldownBaseTicks = 18,
                MinAttackSpeed = 0.20f,
                MaxAttackSpeed = 3.00f,
                MinChargeSpeed = 0.20f,
                MaxChargeSpeed = 3.00f
            };
        }
        
        [System.Serializable]
        private struct CombatAnimationSection
        {
            public int LightAttackDurationBaseTicks;
            public int HeavyAttackDurationBaseTicks;

            [Min(0.01f)] public float MinAnimatorSpeed;
            [Min(0.01f)] public float MaxAnimatorSpeed;

            public static CombatAnimationSection Default => new CombatAnimationSection
            {
                LightAttackDurationBaseTicks = 16,
                HeavyAttackDurationBaseTicks = 22,
                MinAnimatorSpeed = 0.25f,
                MaxAnimatorSpeed = 3.5f
            };
        }

        [System.Serializable]
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

        [System.Serializable]
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

        [System.Serializable]
        private struct PhysicsProbesSection
        {
            [Header("Layer Masks (multi-select)")]
            public LayerMask GroundLayers; // несколько слоёв для grounded-check
            public LayerMask WallLayers;   // несколько слоёв для wall-check

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

        [System.Serializable]
        private struct PhysicsWorldSection
        {
            public int MaxSubSteps;

            public static PhysicsWorldSection Default => new PhysicsWorldSection
            {
                MaxSubSteps = 8
            };
        }
    }
}