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

        private LayerController _attackLayer;
        private LayerController _defenseLayer;
        private LayerController _evadeLayer;

        private bool _prevChargeFull;
        private int _lastTriggerActionTick = int.MinValue;

        public void Initialize(Animator animator, ChargeFullFlashView flash, AttackAnimationConfigAsset config)
        {
            _animator = animator;
            _flash = flash;
            _config = config;
            _h = AnimatorHashes.Create();

            if (_config == null)
                throw new ArgumentNullException(nameof(config), "AttackAnimationConfigAsset is required for EntityAnimatorPresenter.");

            if (_animator == null)
                return;

            _attackLayer = new LayerController();
            _defenseLayer = new LayerController();
            _evadeLayer = new LayerController();

            _attackLayer.Initialize(
                _animator,
                _config.AttackLayerName,
                _config.AttackStateTag,
                _config.AttackLayerBlendInSeconds,
                _config.AttackLayerBlendOutSeconds);

            _defenseLayer.Initialize(
                _animator,
                _config.DefenseLayerName,
                _config.DefenseStateTag,
                _config.DefenseLayerBlendInSeconds,
                _config.DefenseLayerBlendOutSeconds);

            // Optional (can be absent in Animator Controller)
            _evadeLayer.Initialize(
                _animator,
                _config.EvadeLayerName,
                _config.EvadeStateTag,
                _config.EvadeLayerBlendInSeconds,
                _config.EvadeLayerBlendOutSeconds);
        }

        public void Present(AnimationState a, int facing, float dt, float fixedDt)
        {
            if (_animator == null) return;

            // Base locomotion params (Layer0)
            _animator.SetBool(_h.Grounded, a.Grounded);
            _animator.SetBool(_h.JustLanded, a.JustLanded);
            _animator.SetBool(_h.Moving, a.Moving);
            _animator.SetFloat(_h.Speed01, a.Speed01);
            _animator.SetFloat(_h.AirSpeed01, a.AirSpeed01);
            _animator.SetFloat(_h.AirT, a.AirT);

            // Defense signal for animation graph (block pose on Defense layer)
            _animator.SetBool(_h.Blocking, a.Blocking);

            // Charge (Attack layer usually)
            _animator.SetBool(_h.HeavyCharge, a.HeavyCharging);
            _animator.SetFloat(_h.Charge01, a.Charge01);
            _animator.SetFloat(_h.ChargeAnimSpeed, a.ChargeAnimSpeed);
            SyncChargeFx(a.Charge01, facing);

            // Layer weights (Attack/Defense/Evade)
            ApplyLayerWeights(a, dt);

            // Stable action speed while PlayingAction is active
            float effectiveActionSpeed = ResolveActionAnimSpeed(a, fixedDt);
            _animator.SetFloat(_h.AttackAnimSpeed, effectiveActionSpeed);

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

        private void ApplyLayerWeights(AnimationState a, float dt)
        {
            bool wantsAttack =
                a.HeavyCharging ||
                a.PlayingAction == ActionState.LightAttack ||
                a.PlayingAction == ActionState.HeavyAttack;

            bool wantsDefense =
                a.Blocking ||
                a.PlayingAction == ActionState.Parry;

            bool wantsEvade =
                a.PlayingAction == ActionState.Dodge ||
                a.PlayingAction == ActionState.DodgePerfect;

            bool hasEvadeLayer = _evadeLayer.IsEnabled;

            // If there is NO separate Evade layer, allow Dodge to live in Defense layer.
            if (!hasEvadeLayer && wantsEvade)
                wantsDefense = true;

            // Priority: Evade(full-body) should dominate (avoid mixing with upper-body layers)
            bool evadeInTag = hasEvadeLayer && _evadeLayer.IsPlayingTagged(_animator);
            bool evadeDominant = hasEvadeLayer && (wantsEvade || evadeInTag);

            if (evadeDominant)
            {
                _evadeLayer.Update(_animator, wantsEvade, dt);
                _attackLayer.Update(_animator, false, dt);
                _defenseLayer.Update(_animator, false, dt);
                return;
            }

            _attackLayer.Update(_animator, wantsAttack, dt);
            _defenseLayer.Update(_animator, wantsDefense, dt);
            _evadeLayer.Update(_animator, wantsEvade, dt);
        }

        private float ResolveActionAnimSpeed(AnimationState a, float fixedDt)
        {
            if (a.PlayingAction != ActionState.None && a.PlayingDurationTicks > 0)
                return ComputeForcedAnimatorSpeed(a.PlayingAction, a.PlayingDurationTicks, fixedDt, a.AttackAnimSpeed);

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

        private sealed class LayerController
        {
            private int _index = -1;
            private int _tagHash;
            private float _weight;
            private float _blendIn;
            private float _blendOut;

            public bool IsEnabled => _index >= 0;

            public void Initialize(
                Animator animator,
                string layerName,
                string stateTag,
                float blendInSeconds,
                float blendOutSeconds)
            {
                _index = -1;
                _tagHash = 0;
                _weight = 0f;

                if (animator == null) return;
                if (string.IsNullOrEmpty(layerName)) return;

                int idx = animator.GetLayerIndex(layerName);
                if (idx < 0) return;

                _index = idx;

                // Important: empty tag would hash to 0 and match "Untagged" => disable tag checks when empty
                _tagHash = string.IsNullOrEmpty(stateTag) ? 0 : Animator.StringToHash(stateTag);

                _blendIn = blendInSeconds < 0f ? 0f : blendInSeconds;
                _blendOut = blendOutSeconds < 0f ? 0f : blendOutSeconds;

                animator.SetLayerWeight(_index, 0f);
            }

            public void Update(Animator animator, bool wants, float dt)
            {
                if (_index < 0) return;
                if (animator == null) return;
                if (_index >= animator.layerCount) return;

                bool inTag = IsPlayingTagged(animator);
                float target = (wants || inTag) ? 1f : 0f;

                float tau = target > _weight ? _blendIn : _blendOut;
                _weight = Damp01(_weight, target, tau, dt);

                animator.SetLayerWeight(_index, _weight);
            }

            public bool IsPlayingTagged(Animator animator)
            {
                if (_index < 0) return false;
                if (animator == null) return false;
                if (_index >= animator.layerCount) return false;
                if (_tagHash == 0) return false;

                var cur = animator.GetCurrentAnimatorStateInfo(_index);
                if (cur.tagHash == _tagHash)
                    return true;

                if (!animator.IsInTransition(_index))
                    return false;

                var next = animator.GetNextAnimatorStateInfo(_index);
                return next.tagHash == _tagHash;
            }
        }
    }
}