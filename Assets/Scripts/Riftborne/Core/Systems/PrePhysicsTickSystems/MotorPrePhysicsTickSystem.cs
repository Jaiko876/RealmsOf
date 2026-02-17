using Riftborne.Core.Input;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Core.Physics.Model;
using Riftborne.Core.Simulation;
using Riftborne.Core.Stores;

namespace Riftborne.Core.Systems.PrePhysicsTickSystems
{
    public sealed class MotorPrePhysicsTickSystem : IPrePhysicsTickSystem
    {
        private readonly IBodyProvider<GameEntityId> _bodies;
        private readonly IGroundSensor _ground;
        private readonly ICharacterMotor _motor;
        private readonly MotorParams _motorParams;
        private readonly SimulationParameters _parameters;
        private readonly IMotorInputStore _motorInputs;

        public MotorPrePhysicsTickSystem(
            IBodyProvider<GameEntityId> bodies,
            IGroundSensor ground,
            ICharacterMotor motor,
            MotorParams motorParams,
            SimulationParameters parameters,
            IMotorInputStore motorInputs)
        {
            _bodies = bodies;
            _ground = ground;
            _motor = motor;
            _motorParams = motorParams;
            _parameters = parameters;
            _motorInputs = motorInputs;
        }

        public void Tick(int tick)
        {
            // Вариант 1: если ты можешь перечислить все тела:
            foreach (var entityId in _bodies.EnumerateIds()) // добавь это в IBodyProvider, если нет
            {
                if (!_bodies.TryGet(entityId, out var body))
                    continue;

                var grounded = _ground.IsGrounded(entityId);

                // если ввода нет — считаем нулевой (важно!)
                if (!_motorInputs.TryGet(entityId, out var input))
                    input = MotorInput.None(entityId);

                var modifiers = PhysicsModifiers.None; // потом сюда slow/root/stun

                var ctx = new MotorContext(
                    _parameters.TickDeltaTime,
                    body,
                    _motorParams,
                    modifiers,
                    grounded
                );

                _motor.Apply(input, ctx);
            }

            _motorInputs.Clear();
        }
    }
}
