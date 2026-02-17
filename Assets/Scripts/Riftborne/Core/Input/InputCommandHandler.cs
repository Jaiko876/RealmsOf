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

            var input = new MotorInput(
                command.EntityId,
                command.Dx,
                jumpPressed,
                jumpHeld
            );

            _motorInputs.Set(command.EntityId, input);
        }
    }
}