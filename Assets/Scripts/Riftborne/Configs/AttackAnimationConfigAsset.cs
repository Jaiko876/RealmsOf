using Riftborne.Core.Model.Animation;
using UnityEngine;

namespace Riftborne.Configs
{
    [CreateAssetMenu(
        fileName = "AttackAnimationConfig",
        menuName = "Riftborne/Config/Animation/Attack Animation Config",
        order = 0)]
    public sealed class AttackAnimationConfigAsset : ScriptableObject
    {
        [Header("Animator Layers (names must match Animator Controller)")]
        [SerializeField] private string attackLayerName = "Attack Layer";
        [SerializeField] private string defenseLayerName = "Defense Layer";
        [SerializeField] private string evadeLayerName = "Evade Layer"; // optional

        [Header("State Tags (per-layer)")]
        [SerializeField] private string attackStateTag = "Attack";
        [SerializeField] private string defenseStateTag = "Defense";
        [SerializeField] private string evadeStateTag = "Evade";

        [Header("Layer Blend (seconds)")]
        [SerializeField] private float attackLayerBlendInSeconds = 0.05f;
        [SerializeField] private float attackLayerBlendOutSeconds = 0.25f;
        [SerializeField] private float defenseLayerBlendInSeconds = 0.05f;
        [SerializeField] private float defenseLayerBlendOutSeconds = 0.20f;
        [SerializeField] private float evadeLayerBlendInSeconds = 0.02f;
        [SerializeField] private float evadeLayerBlendOutSeconds = 0.10f;

        [Header("Action Clips (seconds at speed=1)")]
        [SerializeField] private float lightAttackClipSeconds = 0.45f;
        [SerializeField] private float heavyAttackClipSeconds = 0.60f;
        [SerializeField] private float parryClipSeconds = 0.35f;
        [SerializeField] private float dodgeClipSeconds = 0.45f;
        [SerializeField] private float dodgePerfectClipSeconds = 0.25f;

        [Header("Animator Speed Clamp")]
        [SerializeField] private float minAnimatorSpeed = 0.25f;
        [SerializeField] private float maxAnimatorSpeed = 3.5f;

        public string AttackLayerName => attackLayerName;
        public string DefenseLayerName => defenseLayerName;
        public string EvadeLayerName => evadeLayerName;

        public string AttackStateTag => attackStateTag;
        public string DefenseStateTag => defenseStateTag;
        public string EvadeStateTag => evadeStateTag;

        public float AttackLayerBlendInSeconds => attackLayerBlendInSeconds;
        public float AttackLayerBlendOutSeconds => attackLayerBlendOutSeconds;

        public float DefenseLayerBlendInSeconds => defenseLayerBlendInSeconds;
        public float DefenseLayerBlendOutSeconds => defenseLayerBlendOutSeconds;

        public float EvadeLayerBlendInSeconds => evadeLayerBlendInSeconds;
        public float EvadeLayerBlendOutSeconds => evadeLayerBlendOutSeconds;

        public float MinAnimatorSpeed => minAnimatorSpeed;
        public float MaxAnimatorSpeed => maxAnimatorSpeed;

        public float GetClipSeconds(ActionState action)
        {
            if (action == ActionState.LightAttack) return lightAttackClipSeconds;
            if (action == ActionState.HeavyAttack) return heavyAttackClipSeconds;
            if (action == ActionState.Parry) return parryClipSeconds;
            if (action == ActionState.Dodge) return dodgeClipSeconds;
            if (action == ActionState.DodgePerfect) return dodgePerfectClipSeconds;
            return 0f;
        }

        private void OnValidate()
        {
            attackLayerBlendInSeconds = Mathf.Max(0f, attackLayerBlendInSeconds);
            attackLayerBlendOutSeconds = Mathf.Max(0f, attackLayerBlendOutSeconds);

            defenseLayerBlendInSeconds = Mathf.Max(0f, defenseLayerBlendInSeconds);
            defenseLayerBlendOutSeconds = Mathf.Max(0f, defenseLayerBlendOutSeconds);

            evadeLayerBlendInSeconds = Mathf.Max(0f, evadeLayerBlendInSeconds);
            evadeLayerBlendOutSeconds = Mathf.Max(0f, evadeLayerBlendOutSeconds);

            lightAttackClipSeconds = Mathf.Max(0f, lightAttackClipSeconds);
            heavyAttackClipSeconds = Mathf.Max(0f, heavyAttackClipSeconds);
            parryClipSeconds = Mathf.Max(0f, parryClipSeconds);
            dodgeClipSeconds = Mathf.Max(0f, dodgeClipSeconds);
            dodgePerfectClipSeconds = Mathf.Max(0f, dodgePerfectClipSeconds);

            if (maxAnimatorSpeed < minAnimatorSpeed)
                maxAnimatorSpeed = minAnimatorSpeed;
        }
    }
}