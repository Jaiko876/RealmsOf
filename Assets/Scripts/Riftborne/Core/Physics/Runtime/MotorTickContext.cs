using Riftborne.Core.Gameplay.Physics.Modifiers;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Core.Physics.Model;

namespace Riftborne.Core.Physics.Runtime
{
    public readonly struct MotorTickContext
    {
        public readonly float Dt;
        public readonly IPhysicsBody Body;
        public readonly MotorParams Params;
        public readonly PhysicsModifiers Modifiers;
        public readonly bool IsGrounded;

        public readonly bool BlockedLeft;
        public readonly bool BlockedRight;

        public MotorTickContext(
            float dt,
            IPhysicsBody body,
            MotorParams @params,
            PhysicsModifiers modifiers,
            bool isGrounded,
            bool blockedLeft,
            bool blockedRight)
        {
            Dt = dt;
            Body = body;
            Params = @params;
            Modifiers = modifiers;
            IsGrounded = isGrounded;
            BlockedLeft = blockedLeft;
            BlockedRight = blockedRight;
        }
    }
}