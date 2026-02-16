using System;
using System.Collections.Generic;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Core.Physics.Model;

namespace Riftborne.Physics
{
    public sealed class PlatformerCharacterMotor : ICharacterMotor
    {
        private readonly Dictionary<int, float> _coyote = new();
        private readonly Dictionary<int, float> _jumpBuffer = new();

        public void Apply(MotorInput input, in MotorContext ctx)
        {
            var dt = ctx.Dt;
            var p = ctx.Params;
            var body = ctx.Body;
            int pid = input.EntityId.Value;

            // --- Horizontal ---
            float targetVx = Clamp(input.MoveX, -1f, 1f) * p.MaxSpeedX;
            float accel = Math.Abs(targetVx) > Math.Abs(body.Vx) ? p.AccelX : p.DecelX;
            body.Vx = MoveTowards(body.Vx, targetVx, accel * dt);

            // --- Timers ---
            float coyote = Get(_coyote, pid);
            float buffer = Get(_jumpBuffer, pid);

            coyote = ctx.IsGrounded
                ? p.CoyoteTimeSeconds
                : Math.Max(0f, coyote - dt);

            buffer = input.JumpPressed
                ? p.JumpBufferSeconds
                : Math.Max(0f, buffer - dt);

            // --- Jump decision ---
            bool jumpNow =
                (ctx.IsGrounded && input.JumpPressed) ||
                (buffer > 0f && coyote > 0f);

            if (jumpNow)
            {
                buffer = 0f;
                coyote = 0f;
                body.Vy = p.JumpVelocity;
            }

            _coyote[pid] = coyote;
            _jumpBuffer[pid] = buffer;

            // --- Chaos modifiers ---
            if (ctx.Modifiers.ImpulseX != 0f || ctx.Modifiers.ImpulseY != 0f)
                body.AddImpulse(ctx.Modifiers.ImpulseX, ctx.Modifiers.ImpulseY);
        }

        private static float Get(Dictionary<int, float> d, int k)
            => d.TryGetValue(k, out var v) ? v : 0f;

        private static float Clamp(float v, float min, float max)
            => v < min ? min : (v > max ? max : v);

        private static float MoveTowards(float current, float target, float maxDelta)
        {
            if (Math.Abs(target - current) <= maxDelta)
                return target;

            return current + Math.Sign(target - current) * maxDelta;
        }
    }
}
