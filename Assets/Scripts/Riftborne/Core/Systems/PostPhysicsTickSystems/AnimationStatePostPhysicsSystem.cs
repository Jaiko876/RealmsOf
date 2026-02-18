using System;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Model;

namespace Riftborne.Core.Systems.PostPhysicsTickSystems
{
    public sealed class AnimationStatePostPhysicsSystem : IPostPhysicsTickSystem
    {
        private readonly GameState _state;
        private readonly MotorParams _motor;

        // Подстрой по ощущениям (0.01..0.03 обычно норм)
        private const float SpeedDeadZone01 = 0.01f;

        public AnimationStatePostPhysicsSystem(GameState state, MotorParams motor)
        {
            _state = state;
            _motor = motor;
        }

        public void Tick(int tick)
        {
            foreach (var kv in _state.Entities)
            {
                var e = kv.Value;
                var a = e.AnimationState;

                a.Facing = (sbyte)(e.Facing < 0 ? -1 : 1);

                a.Grounded = e.Grounded;
                a.JustLanded = (!e.PrevGrounded && e.Grounded);

                // Speed01
                var speedAbs = Math.Abs(e.Vx);
                a.Speed01 = Normalize01(speedAbs, _motor.MaxSpeedX);
                a.Speed01 = ApplyDeadZone01(a.Speed01, SpeedDeadZone01);
                
                a.Moving = a.Speed01 > 0f;

                // Air params
                if (e.Grounded)
                {
                    a.AirSpeed01 = 0f;
                    a.AirT = 1f;
                }
                else
                {
                    a.AirSpeed01 = a.Speed01;

                    // Apex (Vy=0) -> 0.5, вверх -> (0..0.5), вниз -> (0.5..1)
                    a.AirT = ComputeAirT(e.Vy, _motor.JumpVelocity, _motor.MaxFallSpeed);
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

        /// <summary>
        /// AirT in [0..1]:
        /// 0   = сильный подъём (Vy ~ +jumpVel)
        /// 0.5 = вершина (Vy ~ 0)
        /// 1   = сильное падение (Vy ~ -maxFall)
        /// </summary>
        private static float ComputeAirT(float vy, float jumpVel, float maxFall)
        {
            if (jumpVel < 0.0001f) jumpVel = 0.0001f;
            if (maxFall < 0.0001f) maxFall = 0.0001f;

            if (vy >= 0f)
            {
                // Up: vy in [0..jumpVel] => t in [0.5..0]
                var u = Clamp01(vy / jumpVel);      // 0..1
                return 0.5f * (1f - u);             // 0.5..0
            }
            else
            {
                // Down: vy in [-maxFall..0] => t in [1..0.5]
                var d = Clamp01((-vy) / maxFall);   // 0..1
                return 0.5f + 0.5f * d;             // 0.5..1
            }
        }

        private static float Clamp01(float v)
        {
            if (v < 0f) return 0f;
            if (v > 1f) return 1f;
            return v;
        }
    }
}
