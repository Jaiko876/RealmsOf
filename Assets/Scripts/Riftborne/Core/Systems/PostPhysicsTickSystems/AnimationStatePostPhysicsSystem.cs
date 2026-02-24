using System;
using Riftborne.Core.Animation.Abstractions;
using Riftborne.Core.Config;
using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;
using Riftborne.Core.Physics.Model;
using Riftborne.Core.Stores;

namespace Riftborne.Core.Systems.PostPhysicsTickSystems
{
    public sealed class AnimationStatePostPhysicsSystem : IPostPhysicsTickSystem
    {
        private readonly GameState _state;
        private readonly MotorParams _motor;
        private readonly IActionIntentStore _actions;
        private readonly IActionTimingStore _timings;
        private readonly IAttackChargeStore _charge;
        private readonly IAnimationModifiersProvider _animMods;
        private readonly InputTuning _inputTuning;

        public AnimationStatePostPhysicsSystem(
            GameState state,
            MotorParams motor,
            IActionIntentStore actions,
            IActionTimingStore timings,
            IAttackChargeStore charge,
            IAnimationModifiersProvider animMods,
            IGameplayTuning tuning)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _motor = motor ?? throw new ArgumentNullException(nameof(motor));
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _timings = timings ?? throw new ArgumentNullException(nameof(timings));
            _charge = charge ?? throw new ArgumentNullException(nameof(charge));
            _animMods = animMods ?? throw new ArgumentNullException(nameof(animMods));
            _inputTuning = (tuning ?? throw new ArgumentNullException(nameof(tuning))).Input;
        }

        public void Tick(int tick)
        {
            float speedDeadZone01 = _inputTuning.MoveSpeedDeadzone01;

            foreach (var kv in _state.Entities)
            {
                var e = kv.Value;
                var a = e.AnimationState;

                a.Facing = (sbyte)(e.Facing < 0 ? -1 : 1);

                a.Grounded = e.Grounded;
                a.JustLanded = (!e.PrevGrounded && e.Grounded);

                var speedAbs = Math.Abs(e.Vx);
                a.Speed01 = Normalize01(speedAbs, _motor.MaxSpeedX);
                a.Speed01 = ApplyDeadZone01(a.Speed01, speedDeadZone01);

                a.Moving = a.Speed01 > 0f;

                if (e.Grounded)
                {
                    a.AirSpeed01 = 0f;
                    a.AirT = 1f;
                }
                else
                {
                    a.AirSpeed01 = a.Speed01;
                    a.AirT = ComputeAirT(e.Vy, _motor.JumpVelocity, _motor.MaxFallSpeed);
                }

                if (_charge.TryGet(e.Id, out var charging, out var charge01))
                {
                    a.HeavyCharging = charging;
                    a.Charge01 = charge01;
                }
                else
                {
                    a.HeavyCharging = false;
                    a.Charge01 = 0f;
                }

                var mods = _animMods.Get(e.Id);
                a.AttackAnimSpeed = mods.AttackAnimSpeed;
                a.ChargeAnimSpeed = mods.ChargeAnimSpeed;

                // IMPORTANT: Action + ActionDurationTicks are "event payload".
                if (_actions.TryConsume(e.Id, out var act))
                {
                    a.Action = act;
                    a.ActionTick = tick;

                    a.ActionDurationTicks = 0;

                    // Timing is optional; if absent => view uses AttackAnimSpeed as-is.
                    if (_timings.TryConsume(e.Id, out var timedAct, out var durationTicks) && timedAct == act)
                    {
                        a.ActionDurationTicks = durationTicks;
                    }
                }
                else
                {
                    a.Action = ActionState.None;
                    // ActionTick не трогаем (edge-detect во view)
                    a.ActionDurationTicks = 0;
                }

                e.SetAnimationState(a);
            }
        }

        private static float Normalize01(float value, float max)
        {
            if (max <= 0.0001f) return 0f;
            return Clamp01(value / max);
        }

        private static float ApplyDeadZone01(float v, float eps)
        {
            if (v < eps) return 0f;
            return v;
        }

        private static float ComputeAirT(float vy, float jumpVel, float maxFall)
        {
            if (jumpVel < 0.0001f) jumpVel = 0.0001f;
            if (maxFall < 0.0001f) maxFall = 0.0001f;

            if (vy >= 0f)
            {
                var u = Clamp01(vy / jumpVel);
                return 0.5f * (1f - u);
            }

            var d = Clamp01((-vy) / maxFall);
            return 0.5f + 0.5f * d;
        }

        private static float Clamp01(float v)
        {
            if (v < 0f) return 0f;
            if (v > 1f) return 1f;
            return v;
        }
    }
}