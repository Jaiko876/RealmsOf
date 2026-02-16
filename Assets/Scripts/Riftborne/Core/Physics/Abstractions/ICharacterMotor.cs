using Riftborne.Core.Physics.Model;

namespace Riftborne.Core.Physics.Abstractions
{

    public interface ICharacterMotor
    {
        void Apply(MotorInput input, in MotorContext ctx);
    }
}
