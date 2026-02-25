using System;
using Riftborne.App.Animation.Composition.Abstractions;
using Riftborne.App.Animation.Utils;
using Riftborne.Core.Model.Animation;

namespace Riftborne.App.Animation.Composition
{
    public sealed class DefaultAnimationStateComposer : IAnimationStateComposer
    {
        public AnimationState Compose(in AnimationStateComposeContext ctx)
        {
            var e = ctx.Entity;
            var a = ctx.Current;

            a.Facing = (sbyte)(e.Facing < 0 ? -1 : 1);

            a.Grounded = e.Grounded;
            a.JustLanded = (!e.PrevGrounded && e.Grounded);

            var speedAbs = Math.Abs(e.Vx);
            a.Speed01 = AnimationMath.Normalize01(speedAbs, ctx.Motor.MaxSpeedX);
            a.Speed01 = AnimationMath.ApplyDeadZone01(a.Speed01, ctx.Input.MoveSpeedDeadzone01);
            a.Moving = a.Speed01 > 0f;

            if (e.Grounded)
            {
                a.AirSpeed01 = 0f;
                a.AirT = 1f;
            }
            else
            {
                a.AirSpeed01 = a.Speed01;
                a.AirT = AnimationMath.ComputeAirT(e.Vy, ctx.Motor.JumpVelocity, ctx.Motor.MaxFallSpeed);
            }

            a.HeavyCharging = ctx.Charge.IsCharging;
            a.Charge01 = ctx.Charge.Charge01;

            a.AttackAnimSpeed = ctx.Mods.AttackAnimSpeed;
            a.ChargeAnimSpeed = ctx.Mods.ChargeAnimSpeed;

            if (ctx.Action.Action != ActionState.None)
            {
                a.Action = ctx.Action.Action;
                a.ActionTick = ctx.Action.ActionTick;
                a.ActionDurationTicks = ctx.Action.DurationTicks;
            }
            else
            {
                a.Action = ActionState.None;
                a.ActionDurationTicks = 0;
            }

            return a;
        }
    }
}