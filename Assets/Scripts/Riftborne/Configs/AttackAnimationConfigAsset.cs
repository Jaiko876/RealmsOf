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
        [Header("Animator Layers")]
        [SerializeField] private string attackLayerName = "Attack Layer";
        [SerializeField] private string attackStateTag = "Attack";
        [SerializeField] private float attackLayerBlendInSeconds = 0.05f;
        [SerializeField] private float attackLayerBlendOutSeconds = 0.25f;

        [Header("Attack Clips (seconds at speed=1)")]
        [SerializeField] private float lightAttackClipSeconds = 0.45f;
        [SerializeField] private float heavyAttackClipSeconds = 0.60f;

        [Header("Animator Speed Clamp")]
        [SerializeField] private float minAnimatorSpeed = 0.25f;
        [SerializeField] private float maxAnimatorSpeed = 3.5f;
        
        [Header("Parry/Dodge Clips")]
        [SerializeField] private float parryClipSeconds = 0.35f;
        [SerializeField] private float dodgeClipSeconds = 0.45f;
        [SerializeField] private float dodgePerfectClipSeconds = 0.25f;

        public string AttackLayerName => attackLayerName;
        public string AttackStateTag => attackStateTag;
        public float AttackLayerBlendInSeconds => attackLayerBlendInSeconds;
        public float AttackLayerBlendOutSeconds => attackLayerBlendOutSeconds;
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

            lightAttackClipSeconds = Mathf.Max(0f, lightAttackClipSeconds);
            heavyAttackClipSeconds = Mathf.Max(0f, heavyAttackClipSeconds);

            if (maxAnimatorSpeed < minAnimatorSpeed)
                maxAnimatorSpeed = minAnimatorSpeed;
        }
    }
}