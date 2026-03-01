using System;
using Riftborne.Core.Config;
using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Core.Physics.Model;
using Riftborne.Core.Stores.Abstractions;
using Riftborne.Core.Systems.PrePhysicsTickSystems;

namespace Riftborne.App.Combat.Systems
{
    /// <summary>
    /// Applies authoritative dodge movement (roll) during Dodge.Active window.
    /// Runs AFTER MotorPrePhysicsTickSystem so it can override Vx deterministically.
    /// </summary>
    public sealed class CombatDodgeMovementPrePhysicsSystem : IPrePhysicsTickSystem
    {
        private readonly IBodyProvider<GameEntityId> _bodies;
        private readonly ICombatActionStore _actions;
        private readonly MotorParams _motor;
        private readonly CombatActionsTuning _tuning;

        public CombatDodgeMovementPrePhysicsSystem(
            IBodyProvider<GameEntityId> bodies,
            ICombatActionStore actions,
            MotorParams motor,
            IGameplayTuning tuning)
        {
            _bodies = bodies ?? throw new ArgumentNullException(nameof(bodies));
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _motor = motor ?? throw new ArgumentNullException(nameof(motor));
            if (tuning == null) throw new ArgumentNullException(nameof(tuning));
            _tuning = tuning.CombatActions;
        }

        public void Tick(int tick)
        {
            foreach (var id in _bodies.EnumerateIds())
            {
                if (!_actions.TryGet(id, out var a))
                    continue;

                if (a.Type != CombatActionType.Dodge)
                    continue;

                if (!a.IsRunningAt(tick))
                    continue;

                if (!a.IsActiveAt(tick))
                    continue;

                if (!_bodies.TryGet(id, out var body))
                    continue;

                sbyte dir = a.LockedFacing != 0 ? a.LockedFacing : (sbyte)1;

                float speed = _motor.MaxSpeedX * _tuning.DodgeMovement.RollSpeedMul;
                body.Vx = dir * speed;
            }
        }
    }
}