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

            // --- One-shot trigger + latched playback window ---
            ApplyAction(in ctx, ref a);

            a.Blocking = ctx.Blocking;

            return a;
        }

        private static void ApplyAction(in AnimationStateComposeContext ctx, ref AnimationState a)
        {
            // 1) One-shot trigger for this tick (used for Animator.SetTrigger)
            if (ctx.Action.Action != ActionState.None)
            {
                a.Action = ctx.Action.Action;
                a.ActionTick = ctx.Action.ActionTick;
                a.ActionDurationTicks = ctx.Action.DurationTicks;

                // 2) Latch playing window if duration is known
                if (ctx.Action.DurationTicks > 0)
                {
                    a.PlayingAction = ctx.Action.Action;
                    a.PlayingStartTick = ctx.Action.ActionTick;
                    a.PlayingDurationTicks = ctx.Action.DurationTicks;
                }
            }
            else
            {
                a.Action = ActionState.None;
                a.ActionDurationTicks = 0;
                // ActionTick intentionally left as-is (useful for edge debugging, and triggers are gated by Action != None anyway)
            }

            // 3) Expire playing window deterministically
            if (a.PlayingAction != ActionState.None && a.PlayingDurationTicks > 0)
            {
                // playing interval: [start, end)
                int endTick = a.PlayingStartTick + a.PlayingDurationTicks;
                if (ctx.Tick >= endTick)
                {
                    a.PlayingAction = ActionState.None;
                    a.PlayingStartTick = 0;
                    a.PlayingDurationTicks = 0;
                }
            }
        }
    }
}