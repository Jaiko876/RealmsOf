using Riftborne.Core.Commands;
using Riftborne.Core.Physics.Model;
using Riftborne.Core.Stores;

namespace Riftborne.Core.Input
{
    public sealed class InputCommandHandler : ICommandHandler<InputCommand>
    {
        private readonly IMotorInputStore _motorInputs;

        public InputCommandHandler(IMotorInputStore motorInputs)
        {
            _motorInputs = motorInputs;
        }

        public void Handle(InputCommand command)
        {
            var jumpPressed = (command.Buttons & InputButtons.JumpPressed) != 0;
            var jumpHeld    = (command.Buttons & InputButtons.JumpHeld) != 0;
            
            sbyte facing = 0;
            const float dead = 0.1f;
            if (command.Dx > dead) facing = 1;
            else if (command.Dx < -dead) facing = -1;

            var input = new MotorInput(
                command.EntityId,
                command.Dx,
                jumpPressed,
                jumpHeld,
                facing
            );

            _motorInputs.Set(command.EntityId, input);
        }
    }
}