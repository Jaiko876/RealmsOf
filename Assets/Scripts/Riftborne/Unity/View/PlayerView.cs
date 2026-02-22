using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;
using Riftborne.Unity.VFX;
using UnityEngine;
using VContainer;
using AnimationState = Riftborne.Core.Model.Animation.AnimationState;

namespace Riftborne.Unity.View
{
    public sealed class PlayerView : MonoBehaviour
    {
        [SerializeField] private int playerId = 0;
        [SerializeField] private int avatarEntityId = 0;

        [Header("View Roots")]
        [SerializeField] private Transform visualRoot;   // интерполяция позиции
        [SerializeField] private Transform flipRoot;     // флип по X (спрайты/аниматор)
        
        [Header("Animation")]
        [SerializeField] private Animator animator;
        [SerializeField] private ChargeFullFlashView _flash;

        private ActionState _prevAction;

        private static readonly int GroundedHash = Animator.StringToHash("Grounded");
        private static readonly int JustLandedHash = Animator.StringToHash("JustLanded");
        private static readonly int Moving = Animator.StringToHash("Moving");
        private static readonly int Speed01Hash = Animator.StringToHash("Speed01");
        private static readonly int AirSpeed01Hash = Animator.StringToHash("AirSpeed01");
        private static readonly int AirTHash = Animator.StringToHash("AirT");
        private static readonly int AtkLightHash = Animator.StringToHash("AtkLight");
        private static readonly int AtkHeavyHash = Animator.StringToHash("AtkHeavy");
        private static readonly int HeavyChargeHash = Animator.StringToHash("HeavyCharge");
        private static readonly int Charge01Hash = Animator.StringToHash("Charge01");

        private GameState _gameState;
        private PlayerId _playerId;
        private GameEntityId _entityId;

        private bool _prevFull;

        [Inject]
        public void Construct(GameState gameState) => _gameState = gameState;

        private void Start()
        {
            _playerId = new PlayerId(playerId);
            _entityId = new GameEntityId(avatarEntityId);

            if (visualRoot == null)
                visualRoot = transform;

            // По умолчанию: если flipRoot не задан — считаем что флипать надо animator.transform (или visualRoot).
            if (flipRoot == null)
            {
                if (animator != null) flipRoot = animator.transform;
                else flipRoot = visualRoot;
            }

            _gameState.PlayerAvatars.Set(_playerId, _entityId);
            _gameState.GetOrCreateEntity(_entityId);
        }

        private void LateUpdate()
        {
            var e = _gameState.GetOrCreateEntity(_entityId);

            // 1) Интерполяция позиции (как было)
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

            // 2) Facing: ФЛИПАЕМ ТОЛЬКО flipRoot (visualRoot не трогаем)
            ApplyFacing(e.Facing);

            // 3) Анимация
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

            if (_flash != null)
            {
                _flash.SetFacing(facing);

                if (full && !_prevFull)
                    _flash.PlayOnce();
            }

            _prevFull = full;
        }

        private void ApplyAnimation(AnimationState a, int facing)
        {
            if (animator == null) return;

            animator.SetBool(GroundedHash, a.Grounded);
            animator.SetBool(JustLandedHash, a.JustLanded);
            animator.SetBool(Moving, a.Moving);

            animator.SetFloat(Speed01Hash, a.Speed01);
            animator.SetFloat(AirSpeed01Hash, a.AirSpeed01);
            animator.SetFloat(AirTHash, a.AirT);

            animator.SetBool(HeavyChargeHash, a.HeavyCharging);
            animator.SetFloat(Charge01Hash, a.Charge01);

            // ВАЖНО: раньше ты всегда передавал 1 -> ломало VFX при флипе
            SyncCharge(a.Charge01, facing);

            if (a.Action != ActionState.None && a.Action != _prevAction)
            {
                animator.ResetTrigger(AtkLightHash);
                animator.ResetTrigger(AtkHeavyHash);

                if (a.Action == ActionState.LightAttack) animator.SetTrigger(AtkLightHash);
                else if (a.Action == ActionState.HeavyAttack) animator.SetTrigger(AtkHeavyHash);
            }

            _prevAction = a.Action;
        }
    }
}