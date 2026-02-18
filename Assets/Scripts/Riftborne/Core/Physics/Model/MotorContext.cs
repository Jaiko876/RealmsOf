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
            MotorParams @params,
            PhysicsModifiers modifiers,
            bool isGrounded)
        {
            Dt = dt;
            Body = body;
            Params = @params;
            Modifiers = modifiers;
            IsGrounded = isGrounded;
        }
    }
}