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

        public CombatInputTuning CombatInput
            => new CombatInputTuning(
                _combatInput.HeavyThresholdBaseTicks,
                _combatInput.FullChargeExtraBaseTicks,
                _combatInput.LightCooldownBaseTicks,
                _combatInput.HeavyCooldownBaseTicks,
                _combatInput.MinAttackSpeed,
                _combatInput.MaxAttackSpeed,
                _combatInput.MinChargeSpeed,
                _combatInput.MaxChargeSpeed);

        public StatsToPhysicsTuning StatsToPhysics
            => new StatsToPhysicsTuning(_statsToPhysics.MinMoveSpeedMultiplier, _statsToPhysics.MaxMoveSpeedMultiplier);

        public InputTuning Input
            => new InputTuning(_input.FacingDeadzone);

        public PhysicsProbesTuning PhysicsProbes
            => new PhysicsProbesTuning(
                new PhysicsProbesTuning.GroundProbeTuning(
                    _physicsProbes.GroundSkin,
                    _physicsProbes.GroundCheckDepth,
                    _physicsProbes.GroundWidthMultiplier,
                    _physicsProbes.GroundProbeHeight),
                new PhysicsProbesTuning.WallProbeTuning(
                    _physicsProbes.WallSkin,
                    _physicsProbes.WallCheckDistance,
                    _physicsProbes.WallProbeThickness,
                    _physicsProbes.WallHeightShrink,
                    _physicsProbes.WallMinWallNormalAbsX));

        public PhysicsWorldTuning PhysicsWorld
            => new PhysicsWorldTuning(_physicsWorld.MaxSubSteps);

        [System.Serializable]
        private struct CombatInputSection
        {
            public int HeavyThresholdBaseTicks;
            public int FullChargeExtraBaseTicks;

            public int LightCooldownBaseTicks;
            public int HeavyCooldownBaseTicks;

            public float MinAttackSpeed;
            public float MaxAttackSpeed;

            public float MinChargeSpeed;
            public float MaxChargeSpeed;

            public static CombatInputSection Default => new CombatInputSection
            {
                HeavyThresholdBaseTicks = 18,
                FullChargeExtraBaseTicks = 42,
                LightCooldownBaseTicks = 18,
                HeavyCooldownBaseTicks = 26,
                MinAttackSpeed = 0.20f,
                MaxAttackSpeed = 3.00f,
                MinChargeSpeed = 0.20f,
                MaxChargeSpeed = 3.00f
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

            public static InputSection Default => new InputSection
            {
                FacingDeadzone = 0.10f
            };
        }

        [System.Serializable]
        private struct PhysicsProbesSection
        {
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