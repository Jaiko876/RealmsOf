using Riftborne.Core.Physics.Model;
using Riftborne.Core.Physics.Runtime;

namespace Riftborne.Core.Physics.Abstractions
{

    public interface ICharacterMotor
    {
        void Apply(MotorInput input, in MotorTickContext ctx);
    }
}
