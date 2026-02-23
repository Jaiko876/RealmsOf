using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Core.Physics.Model;
using Riftborne.Core.Simulation;
using Riftborne.Core.Stores;

namespace Riftborne.Core.Systems.PrePhysicsTickSystems
{
    public sealed class MotorPrePhysicsTickSystem : IPrePhysicsTickSystem
    {
        private readonly GameState _state;
        private readonly IBodyProvider<GameEntityId> _bodies;
        private readonly IGroundSensor _ground;
        private readonly ICharacterMotor _motor;
        private readonly MotorParams _motorParams;
        private readonly SimulationParameters _parameters;
        private readonly IMotorInputStore _motorInputs;
        private readonly IWallSensor _walls;

        private readonly IPhysicsModifiersProvider _modifiers;

        public MotorPrePhysicsTickSystem(
            GameState state,
            IBodyProvider<GameEntityId> bodies,
            IGroundSensor ground,
            ICharacterMotor motor,
            MotorParams motorParams,
            SimulationParameters parameters,
            IMotorInputStore motorInputs,
            IWallSensor walls,
            IPhysicsModifiersProvider modifiers)
        {
            _state = state;
            _bodies = bodies;
            _ground = ground;
            _motor = motor;
            _motorParams = motorParams;
            _parameters = parameters;
            _motorInputs = motorInputs;
            _walls = walls;
            _modifiers = modifiers;
        }

        public void Tick(int tick)
        {
            foreach (var entityId in _bodies.EnumerateIds())
            {
                if (!_bodies.TryGet(entityId, out var body))
                    continue;

                var grounded = _ground.IsGrounded(entityId);

                if (!_motorInputs.TryGet(entityId, out var input))
                    input = MotorInput.None(entityId);

                // ВАЖНО: Facing от намерения, а не от физики
                var e = _state.GetOrCreateEntity(entityId);
                e.ApplyFacingIntent(input.FacingIntent);

                var blockedLeft = _walls.IsBlockedLeft(entityId);
                var blockedRight = _walls.IsBlockedRight(entityId);

                var mods = _modifiers.Get(entityId);

                var ctx = new MotorContext(
                    _parameters.TickDeltaTime,
                    body,
                    _motorParams,
                    mods,
                    grounded,
                    blockedLeft,
                    blockedRight
                );

                _motor.Apply(input, ctx);
            }

            _motorInputs.Clear();
        }
    }
}