using System;
using Riftborne.Configs;
using Riftborne.Core.Model.Animation;
using Riftborne.Unity.VFX;
using Riftborne.Unity.View.Presenters.Abstractions;
using UnityEngine;
using AnimationState = Riftborne.Core.Model.Animation.AnimationState;

namespace Riftborne.Unity.View.Presenters
{
    public sealed class EntityAnimatorPresenter : IEntityAnimatorPresenter
    {
        private Animator _animator;
        private ChargeFullFlashView _flash;
        private AttackAnimationConfigAsset _config;

        private AnimatorHashes _h;

        private int _attackLayerIndex = -1;
        private int _attackTagHash;
        private float _attackLayerWeight;

        private bool _prevChargeFull;
        private int _lastTriggerActionTick = int.MinValue;

        public void Initialize(Animator animator, ChargeFullFlashView flash, AttackAnimationConfigAsset config)
        {
            _animator = animator;
            _flash = flash;
            _config = config;

            _h = AnimatorHashes.Create();

            if (_animator == null)
                return;

            if (_config == null)
                throw new ArgumentNullException(nameof(config), "AttackAnimationConfigAsset is required for EntityAnimatorPresenter.");

            _attackLayerIndex = _animator.GetLayerIndex(_config.AttackLayerName);
            _attackTagHash = Animator.StringToHash(_config.AttackStateTag);

            if (_attackLayerIndex >= 0)
            {
                _attackLayerWeight = 0f;
                _animator.SetLayerWeight(_attackLayerIndex, 0f);
            }
        }

        public void Present(AnimationState a, int facing, float dt, float fixedDt)
        {
            if (_animator == null) return;

            // Base locomotion
            _animator.SetBool(_h.Grounded, a.Grounded);
            _animator.SetBool(_h.JustLanded, a.JustLanded);
            _animator.SetBool(_h.Moving, a.Moving);

            _animator.SetFloat(_h.Speed01, a.Speed01);
            _animator.SetFloat(_h.AirSpeed01, a.AirSpeed01);
            _animator.SetFloat(_h.AirT, a.AirT);

            _animator.SetBool(_h.Blocking, a.Blocking);

            // Charge
            _animator.SetBool(_h.HeavyCharge, a.HeavyCharging);
            _animator.SetFloat(_h.Charge01, a.Charge01);
            _animator.SetFloat(_h.ChargeAnimSpeed, a.ChargeAnimSpeed);

            SyncChargeFx(a.Charge01, facing);

            // Attack layer blend (keep while charging OR playing action OR animator is actually in attack states)
            ApplyAttackLayerWeight(a, dt);

            // Attack speed must be stable while PlayingAction is active
            float effectiveAttackSpeed = ResolveAttackAnimSpeed(a, fixedDt);
            _animator.SetFloat(_h.AttackAnimSpeed, effectiveAttackSpeed);

            // One-shot triggers (ONLY when Action != None)
            bool isNewTrigger = a.Action != ActionState.None && a.ActionTick != _lastTriggerActionTick;
            if (isNewTrigger)
            {
                _lastTriggerActionTick = a.ActionTick;

                if (a.Action == ActionState.LightAttack) _animator.SetTrigger(_h.AtkLight);
                else if (a.Action == ActionState.HeavyAttack) _animator.SetTrigger(_h.AtkHeavy);
                else if (a.Action == ActionState.Parry) _animator.SetTrigger(_h.AtkParry);
                else if (a.Action == ActionState.Dodge) _animator.SetTrigger(_h.AtkDodge);
                else if (a.Action == ActionState.DodgePerfect) _animator.SetTrigger(_h.AtkDodgePerfect);
            }
        }

        private float ResolveAttackAnimSpeed(AnimationState a, float fixedDt)
        {
            // If we have a latched playing window with known duration -> compute forced speed
            if (a.PlayingAction != ActionState.None && a.PlayingDurationTicks > 0)
                return ComputeForcedAnimatorSpeed(a.PlayingAction, a.PlayingDurationTicks, fixedDt, a.AttackAnimSpeed);

            // Otherwise use stat-driven speed
            return ClampAnimatorSpeed(a.AttackAnimSpeed);
        }

        private float ComputeForcedAnimatorSpeed(ActionState action, int durationTicks, float fixedDt, float fallbackSpeed)
        {
            if (durationTicks <= 0)
                return ClampAnimatorSpeed(fallbackSpeed);

            float clipSeconds = _config != null ? _config.GetClipSeconds(action) : 0f;
            if (clipSeconds <= 0f)
                return ClampAnimatorSpeed(fallbackSpeed);

            if (fixedDt <= 0f)
                return ClampAnimatorSpeed(fallbackSpeed);

            float desiredSeconds = durationTicks * fixedDt;
            if (desiredSeconds <= 0f)
                return ClampAnimatorSpeed(fallbackSpeed);

            float speed = clipSeconds / desiredSeconds;
            return ClampAnimatorSpeed(speed);
        }

        private void SyncChargeFx(float charge01, int facing)
        {
            if (_flash == null) return;

            bool full = charge01 >= 0.999f;

            _flash.SetFacing(facing);
            if (full && !_prevChargeFull)
                _flash.PlayOnce();

            _prevChargeFull = full;
        }

        private void ApplyAttackLayerWeight(AnimationState a, float dt)
        {
            if (_attackLayerIndex < 0) return;
            if (_attackLayerIndex >= _animator.layerCount) return;

            bool inAttack = IsAttackPlayingOnLayer();

            bool wants =
                a.HeavyCharging ||
                (a.PlayingAction != ActionState.None) ||
                inAttack;

            float target = wants ? 1f : 0f;

            float tau = target > _attackLayerWeight
                ? _config.AttackLayerBlendInSeconds
                : _config.AttackLayerBlendOutSeconds;

            _attackLayerWeight = Damp01(_attackLayerWeight, target, tau, dt);
            _animator.SetLayerWeight(_attackLayerIndex, _attackLayerWeight);
        }

        private bool IsAttackPlayingOnLayer()
        {
            if (_attackLayerIndex < 0) return false;

            var cur = _animator.GetCurrentAnimatorStateInfo(_attackLayerIndex);
            if (cur.tagHash == _attackTagHash)
                return true;

            if (!_animator.IsInTransition(_attackLayerIndex))
                return false;

            var next = _animator.GetNextAnimatorStateInfo(_attackLayerIndex);
            return next.tagHash == _attackTagHash;
        }

        private float ClampAnimatorSpeed(float v)
        {
            if (_config == null) return v;

            if (v < _config.MinAnimatorSpeed) return _config.MinAnimatorSpeed;
            if (v > _config.MaxAnimatorSpeed) return _config.MaxAnimatorSpeed;
            return v;
        }

        private static float Damp01(float current, float target, float tauSeconds, float dt)
        {
            if (tauSeconds <= 0f) return target;
            if (dt <= 0f) return current;

            float k = 1f - Mathf.Exp(-dt / tauSeconds);
            return Mathf.Lerp(current, target, k);
        }

        private struct AnimatorHashes
        {
            public int Grounded;
            public int JustLanded;
            public int Moving;
            public int Speed01;
            public int AirSpeed01;
            public int AirT;

            public int AtkLight;
            public int AtkHeavy;
            public int AtkParry;
            public int AtkDodge;
            public int AtkDodgePerfect;

            public int HeavyCharge;
            public int Charge01;

            public int AttackAnimSpeed;
            public int ChargeAnimSpeed;

            public int Blocking;

            public static AnimatorHashes Create()
            {
                AnimatorHashes h;
                h.Grounded = Animator.StringToHash("Grounded");
                h.JustLanded = Animator.StringToHash("JustLanded");
                h.Moving = Animator.StringToHash("Moving");

                h.Speed01 = Animator.StringToHash("Speed01");
                h.AirSpeed01 = Animator.StringToHash("AirSpeed01");
                h.AirT = Animator.StringToHash("AirT");

                h.AtkLight = Animator.StringToHash("AtkLight");
                h.AtkHeavy = Animator.StringToHash("AtkHeavy");
                h.AtkParry = Animator.StringToHash("AtkParry");
                h.AtkDodge = Animator.StringToHash("AtkDodge");
                h.AtkDodgePerfect = Animator.StringToHash("AtkDodgePerfect");

                h.HeavyCharge = Animator.StringToHash("HeavyCharge");
                h.Charge01 = Animator.StringToHash("Charge01");

                h.AttackAnimSpeed = Animator.StringToHash("AttackAnimSpeed");
                h.ChargeAnimSpeed = Animator.StringToHash("ChargeAnimSpeed");

                h.Blocking = Animator.StringToHash("Blocking");

                return h;
            }
        }
    }
}