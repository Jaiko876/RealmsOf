using Riftborne.Core.Physics.Abstractions;

namespace Riftborne.Core.Physics.Model
{
    public readonly struct MotorContext
    {
        public readonly float Dt;
        public readonly IPhysicsBody Body;
        public readonly MotorParams Params;
        public readonly PhysicsModifiers Modifiers;
        public readonly bool IsGrounded;

        public MotorContext(
            float dt,
            IPhysicsBody body,
            MotorParams parameters,
            PhysicsModifiers modifiers,
            bool isGrounded)
        {
            Dt = dt;
            Body = body;
            Params = parameters;
            Modifiers = modifiers;
            IsGrounded = isGrounded;
        }
    }
}
