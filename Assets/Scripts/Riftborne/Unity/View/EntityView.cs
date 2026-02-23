using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;
using Riftborne.Unity.VFX;
using UnityEngine;
using VContainer;
using AnimationState = Riftborne.Core.Model.Animation.AnimationState;

namespace Riftborne.Unity.View
{
    public sealed class EntityView : MonoBehaviour
    {
        [Header("Entity Binding")]
        [SerializeField] private int entityId = 0;

        [Header("View Roots")]
        [SerializeField] private Transform visualRoot;
        [SerializeField] private Transform flipRoot;

        [Header("Animation")]
        [SerializeField] private Animator animator;
        [SerializeField] private ChargeFullFlashView flash;

        private static readonly int GroundedHash = Animator.StringToHash("Grounded");
        private static readonly int JustLandedHash = Animator.StringToHash("JustLanded");
        private static readonly int MovingHash = Animator.StringToHash("Moving");
        private static readonly int Speed01Hash = Animator.StringToHash("Speed01");
        private static readonly int AirSpeed01Hash = Animator.StringToHash("AirSpeed01");
        private static readonly int AirTHash = Animator.StringToHash("AirT");

        private static readonly int AtkLightHash = Animator.StringToHash("AtkLight");
        private static readonly int AtkHeavyHash = Animator.StringToHash("AtkHeavy");

        private static readonly int HeavyChargeHash = Animator.StringToHash("HeavyCharge");
        private static readonly int Charge01Hash = Animator.StringToHash("Charge01");

        private static readonly int AttackAnimSpeedHash = Animator.StringToHash("AttackAnimSpeed");
        private static readonly int ChargeAnimSpeedHash = Animator.StringToHash("ChargeAnimSpeed");

        private GameState _gameState;
        private GameEntityId _entityId;

        private bool _prevFull;
        private int _lastActionTick = int.MinValue;

        [Inject]
        public void Construct(GameState gameState) => _gameState = gameState;

        private void Start()
        {
            _entityId = new GameEntityId(entityId);

            if (visualRoot == null)
                visualRoot = transform;

            if (flipRoot == null)
            {
                if (animator != null) flipRoot = animator.transform;
                else flipRoot = visualRoot;
            }

            _gameState.GetOrCreateEntity(_entityId);
        }

        private void LateUpdate()
        {
            var e = _gameState.GetOrCreateEntity(_entityId);

            float alpha = 1f;
            var fd = Time.fixedDeltaTime;
            if (fd > 0f)
            {
                alpha = (Time.time - Time.fixedTime) / fd;
                alpha = Mathf.Clamp01(alpha);
            }

            var x = Mathf.Lerp(e.PrevX, e.X, alpha);
            var y = Mathf.Lerp(e.PrevY, e.Y, alpha);
            visualRoot.position = new Vector3(x, y, 0f);

            ApplyFacing(e.Facing);
            ApplyAnimation(e.AnimationState, e.Facing);
        }

        private void ApplyFacing(int facing)
        {
            if (flipRoot == null) return;

            var s = flipRoot.localScale;
            var ax = Mathf.Abs(s.x);
            s.x = facing < 0 ? -ax : ax;
            flipRoot.localScale = s;
        }

        private void SyncCharge(float charge01, int facing)
        {
            bool full = charge01 >= 0.999f;

            if (flash != null)
            {
                flash.SetFacing(facing);
                if (full && !_prevFull)
                    flash.PlayOnce();
            }

            _prevFull = full;
        }

        private void ApplyAnimation(AnimationState a, int facing)
        {
            if (animator == null) return;

            animator.SetBool(GroundedHash, a.Grounded);
            animator.SetBool(JustLandedHash, a.JustLanded);
            animator.SetBool(MovingHash, a.Moving);

            animator.SetFloat(Speed01Hash, a.Speed01);
            animator.SetFloat(AirSpeed01Hash, a.AirSpeed01);
            animator.SetFloat(AirTHash, a.AirT);

            animator.SetBool(HeavyChargeHash, a.HeavyCharging);
            animator.SetFloat(Charge01Hash, a.Charge01);

            animator.SetFloat(AttackAnimSpeedHash, a.AttackAnimSpeed);
            animator.SetFloat(ChargeAnimSpeedHash, a.ChargeAnimSpeed);

            SyncCharge(a.Charge01, facing);

            if (a.Action != ActionState.None && a.ActionTick != _lastActionTick)
            {
                _lastActionTick = a.ActionTick;

                animator.ResetTrigger(AtkLightHash);
                animator.ResetTrigger(AtkHeavyHash);

                if (a.Action == ActionState.LightAttack) animator.SetTrigger(AtkLightHash);
                else if (a.Action == ActionState.HeavyAttack) animator.SetTrigger(AtkHeavyHash);
            }
        }
    }
}