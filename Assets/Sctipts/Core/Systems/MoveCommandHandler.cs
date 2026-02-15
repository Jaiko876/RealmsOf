using Game.Core.Abstractions;
using Game.Core.Commands;
using Game.Core.Model;
using Game.Core.Physics.Abstractions;
using Game.Core.Physics.Model;
using Game.Core.Simulation;

namespace Game.Core.Systems
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
