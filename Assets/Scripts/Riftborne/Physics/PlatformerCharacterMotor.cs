using System;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Core.Physics.Model;
using Riftborne.Core.Stores;

namespace Riftborne.Physics
{
    public sealed class PlatformerCharacterMotor : ICharacterMotor
    {
        private readonly IMotorStateStore _stateStore;

        public PlatformerCharacterMotor(IMotorStateStore stateStore)
        {
            _stateStore = stateStore;
        }

        public void Apply(MotorInput input, in MotorContext ctx)
        {
            var p = ctx.Params;
            var body = ctx.Body;
            var id = input.EntityId;
            
            float targetVx = Clamp(input.MoveX, -1f, 1f) * p.MaxSpeedX;
            float accel = Math.Abs(targetVx) > Math.Abs(body.Vx) ? p.AccelX : p.DecelX;


            body.Vx = MoveTowards(body.Vx, targetVx, accel * ctx.Dt);


            var s = _stateStore.GetOrCreate(id);

            if (ctx.IsGrounded)
                s.CoyoteTicks = p.CoyoteTicks;
            else if (s.CoyoteTicks > 0)
                s.CoyoteTicks--;

            if (input.JumpPressed)
                s.JumpBufferTicks = p.JumpBufferTicks;
            else if (s.JumpBufferTicks > 0)
                s.JumpBufferTicks--;


            bool jumpNow =
                (ctx.IsGrounded && input.JumpPressed) ||
                (s.JumpBufferTicks > 0 && s.CoyoteTicks > 0);

            if (jumpNow)
            {
                s.JumpBufferTicks = 0;
                s.CoyoteTicks = 0;
                body.Vy = p.JumpVelocity;
            }

            _stateStore.Set(s);
            
            if (ctx.Modifiers.ImpulseX != 0f || ctx.Modifiers.ImpulseY != 0f)
                body.AddImpulse(ctx.Modifiers.ImpulseX, ctx.Modifiers.ImpulseY);
        }

        private static float Clamp(float v, float min, float max)
            => v < min ? min : (v > max ? max : v);

        private static float MoveTowards(float current, float target, float maxDelta)
        {
            var diff = target - current;
            if (Math.Abs(diff) <= maxDelta)
                return target;

            return current + Math.Sign(diff) * maxDelta;
        }
    }
}
