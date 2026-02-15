using Game.Core.Physics.Model;

namespace Game.Core.Physics.Abstractions
{

    public interface ICharacterMotor
    {
        void Apply(MotorInput input, in MotorContext ctx);
    }
}
