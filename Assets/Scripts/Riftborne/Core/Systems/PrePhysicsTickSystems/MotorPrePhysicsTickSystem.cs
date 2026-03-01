using Riftborne.Core.Gameplay.Locomotion.Abstractions;
using Riftborne.Core.Gameplay.Locomotion.Model;
using Riftborne.Core.Gameplay.Physics.Modifiers;
using Riftborne.Core.Gameplay.Physics.Providers;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Core.Physics.Model;
using Riftborne.Core.Physics.Runtime;
using Riftborne.Core.Stores.Abstractions;
using Riftborne.Core.TIme;

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
        private readonly ILocomotionConstraintsProvider _locomotion;

        public MotorPrePhysicsTickSystem(
            GameState state,
            IBodyProvider<GameEntityId> bodies,
            IGroundSensor ground,
            ICharacterMotor motor,
            MotorParams motorParams,
            SimulationParameters parameters,
            IMotorInputStore motorInputs,
            IWallSensor walls,
            IPhysicsModifiersProvider modifiers,
            ILocomotionConstraintsProvider locomotion)
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
            _locomotion = locomotion;
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

                var e = _state.GetOrCreateEntity(entityId);

                var constraints = _locomotion != null
                    ? _locomotion.Get(entityId, tick)
                    : LocomotionConstraints.None;

                // Facing intent (lock has priority)
                if (constraints.HasFacingLock)
                    e.ApplyFacingIntent(constraints.FacingLock);
                else
                    e.ApplyFacingIntent(input.FacingIntent);

                // NEW: prevent moonwalk when requested by constraints
                input = ApplyMoveRestrictions(input, constraints);

                var blockedLeft = _walls.IsBlockedLeft(entityId);
                var blockedRight = _walls.IsBlockedRight(entityId);

                var mods = _modifiers.Get(entityId);
                mods = ApplyLocomotionMultipliers(mods, constraints);

                var ctx = new MotorTickContext(
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

        private static MotorInput ApplyMoveRestrictions(MotorInput input, LocomotionConstraints c)
        {
            if (!c.HasFacingLock || !c.ForbidMoveAgainstFacing)
                return input;

            float mx = input.MoveX;

            if (c.FacingLock > 0)
            {
                if (mx < 0f) mx = 0f;
            }
            else
            {
                if (mx > 0f) mx = 0f;
            }

            if (mx.Equals(input.MoveX))
                return input;

            return new MotorInput(
                input.EntityId,
                mx,
                input.JumpPressed,
                input.JumpHeld,
                input.FacingIntent);
        }

        private static PhysicsModifiers ApplyLocomotionMultipliers(PhysicsModifiers m, LocomotionConstraints c)
        {
            return new PhysicsModifiers(
                gravityScaleMultiplier: m.GravityScaleMultiplier,
                impulseX: m.ImpulseX,
                impulseY: m.ImpulseY,
                moveSpeedMultiplier: m.MoveSpeedMultiplier * c.MoveSpeedMul,
                accelMultiplier: m.AccelMultiplier * c.AccelMul,
                decelMultiplier: m.DecelMultiplier * c.DecelMul);
        }
    }
}