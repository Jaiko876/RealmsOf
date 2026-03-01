using Riftborne.App.Animation.Events;
using Riftborne.Core.Config;
using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;
using Riftborne.Core.Physics.Model;

namespace Riftborne.App.Animation.Composition
{
    public readonly struct AnimationStateComposeContext
    {
        public readonly int Tick;
        public readonly EntityState Entity;
        public readonly AnimationState Current;
        public readonly MotorParams Motor;
        public readonly InputTuning Input;
        public readonly AnimationModifiers Mods;
        public readonly AttackChargeSnapshot Charge;
        public readonly ActionPayload Action;
        public readonly bool Blocking;

        public AnimationStateComposeContext(
            int tick,
            EntityState entity,
            AnimationState current,
            MotorParams motor,
            InputTuning input,
            AnimationModifiers mods,
            AttackChargeSnapshot charge,
            ActionPayload action, 
            bool blocking)
        {
            Tick = tick;
            Entity = entity;
            Current = current;
            Motor = motor;
            Input = input;
            Mods = mods;
            Charge = charge;
            Action = action;
            Blocking = blocking;
        }
    }

    public readonly struct AttackChargeSnapshot
    {
        public readonly bool IsCharging;
        public readonly float Charge01;

        public AttackChargeSnapshot(bool isCharging, float charge01)
        {
            IsCharging = isCharging;
            Charge01 = charge01;
        }

        public static AttackChargeSnapshot None => new AttackChargeSnapshot(false, 0f);
    }
}