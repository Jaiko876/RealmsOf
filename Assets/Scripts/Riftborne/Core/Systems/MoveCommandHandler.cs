using Riftborne.Core.Abstractions;
using Riftborne.Core.Commands;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Core.Physics.Model;
using Riftborne.Core.Simulation;

namespace Riftborne.Core.Systems
{
    public sealed class MoveCommandHandler : ICommandHandler<MoveCommand>
    {
        private readonly IBodyProvider<GameEntityId> _bodies;
        private readonly IGroundSensor _ground;
        private readonly ICharacterMotor _motor;
        private readonly MotorParams _motorParams;
        private readonly SimulationParameters _parameters;

        public MoveCommandHandler(
            IBodyProvider<GameEntityId> bodies,
            IGroundSensor ground,
            ICharacterMotor motor,
            MotorParams motorParams,
            SimulationParameters parameters)
        {
            _bodies = bodies;
            _ground = ground;
            _motor = motor;
            _motorParams = motorParams;
            _parameters = parameters;
        }

        public void Handle(MoveCommand command)
        {
            if (!_bodies.TryGet(command.EntityId, out var body))
                return;

            var grounded = _ground.IsGrounded(command.EntityId);

            var input = new MotorInput(
                command.EntityId,
                command.Dx,
                command.JumpPressed,
                command.JumpHeld
            );

            var modifiers = PhysicsModifiers.None;

            var ctx = new MotorContext(
                _parameters.TickDeltaTime,
                body,
                _motorParams,
                modifiers,
                grounded
            );

            _motor.Apply(input, ctx);
        }
    }
}
