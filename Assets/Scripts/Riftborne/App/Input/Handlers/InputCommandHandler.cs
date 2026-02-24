using System;
using Riftborne.Core.Config;
using Riftborne.Core.Input;
using Riftborne.Core.Input.Commands;
using Riftborne.Core.Input.Handlers;
using Riftborne.Core.Physics.Model;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.App.Input.Handlers
{
    public sealed class InputCommandHandler : ICommandHandler<InputCommand>
    {
        private readonly IMotorInputStore _motorInputs;
        private readonly InputTuning _tuning;

        public InputCommandHandler(IMotorInputStore motorInputs, IGameplayTuning gameplayTuning)
        {
            _motorInputs = motorInputs ?? throw new ArgumentNullException(nameof(motorInputs));
            if (gameplayTuning == null) throw new ArgumentNullException(nameof(gameplayTuning));

            _tuning = gameplayTuning.Input;
        }

        public void Handle(InputCommand command)
        {
            var jumpPressed = (command.Buttons & InputButtons.JumpPressed) != 0;
            var jumpHeld = (command.Buttons & InputButtons.JumpHeld) != 0;

            sbyte facing = 0;
            float dead = _tuning.FacingDeadzone;

            if (command.Dx > dead) facing = 1;
            else if (command.Dx < -dead) facing = -1;

            var input = new MotorInput(
                command.EntityId,
                command.Dx,
                jumpPressed,
                jumpHeld,
                facing);

            _motorInputs.Set(command.EntityId, input);
        }
    }
}