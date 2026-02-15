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
        private readonly IPlayerBodyProvider _bodies;
        private readonly IGroundSensor _ground;
        private readonly ICharacterMotor _motor;
        private readonly MotorParams _motorParams;
        private readonly SimulationParameters _parameters;

        public MoveCommandHandler(
            IPlayerBodyProvider bodies,
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
            if (!_bodies.TryGet(command.PlayerId, out var body))
            {
                return;
            }

            var grounded = _ground.IsGrounded(command.PlayerId);

            var input = new MotorInput(
                command.PlayerId,
                command.Dx,
                command.JumpPressed,
                command.JumpHeld
            );

            // TODO: сюда подключим хаос-слой: modifiers вычисляются детерминированно по тику/сиду
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
