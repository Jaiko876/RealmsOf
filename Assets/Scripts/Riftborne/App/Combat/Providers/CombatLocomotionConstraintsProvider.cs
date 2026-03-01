using System;
using Riftborne.Core.Config;
using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Gameplay.Locomotion.Abstractions;
using Riftborne.Core.Gameplay.Locomotion.Model;
using Riftborne.Core.Model;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.App.Combat.Providers
{
    public sealed class CombatLocomotionConstraintsProvider : ILocomotionConstraintsProvider
    {
        private readonly ICombatActionStore _actions;
        private readonly IAttackChargeStore _charge;
        private readonly CombatActionsTuning _tuning;

        public CombatLocomotionConstraintsProvider(
            ICombatActionStore actions,
            IAttackChargeStore charge,
            IGameplayTuning tuning)
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _charge = charge ?? throw new ArgumentNullException(nameof(charge));
            if (tuning == null) throw new ArgumentNullException(nameof(tuning));
            _tuning = tuning.CombatActions;
        }

        public LocomotionConstraints Get(GameEntityId id, int tick)
        {
            // 1) Active combat action constraints
            if (_actions.TryGet(id, out var a) && a.IsRunningAt(tick))
            {
                return FromAction(in a, tick);
            }

            // 2) Heavy charging (no CombatActionInstance yet)
            // Requirement: slow movement while heavy-charge AND until release
            if (_charge.TryGet(id, out var charging, out var charge01) && charging)
            {
                float mul = _tuning.AttackMovement.HeavyMoveMul;
                return new LocomotionConstraints(
                    hasFacingLock: false,
                    facingLock: 1,
                    forbidMoveAgainstFacing: false,
                    moveSpeedMul: mul,
                    accelMul: mul,
                    decelMul: mul);
            }

            return LocomotionConstraints.None;
        }

        private LocomotionConstraints FromAction(in CombatActionInstance a, int tick)
        {
            sbyte locked = a.LockedFacing != 0 ? a.LockedFacing : (sbyte)1;

            // Defaults
            bool hasFacingLock = false;
            bool forbidBack = false;
            float moveMul = 1f;
            float accelMul = 1f;
            float decelMul = 1f;

            if (a.Type == CombatActionType.LightAttack || a.Type == CombatActionType.HeavyAttack)
            {
                var phase = a.GetPhaseAt(tick);

                // Fix moonwalk:
                // - lock facing only during Windup+Active (commitment window)
                // - forbid moving against facing during that window
                if (phase == CombatPhase.Windup || phase == CombatPhase.Active)
                {
                    hasFacingLock = true;
                    forbidBack = true;

                    moveMul = (a.Type == CombatActionType.HeavyAttack)
                        ? _tuning.AttackMovement.HeavyMoveMul
                        : _tuning.AttackMovement.LightMoveMul;

                    accelMul = moveMul;
                    decelMul = moveMul;
                }
                else
                {
                    // Recovery: back to normal (both movement & turning)
                    hasFacingLock = false;
                    forbidBack = false;
                }

                return new LocomotionConstraints(
                    hasFacingLock: hasFacingLock,
                    facingLock: locked,
                    forbidMoveAgainstFacing: forbidBack,
                    moveSpeedMul: moveMul,
                    accelMul: accelMul,
                    decelMul: decelMul);
            }

            // Parry: lock facing (short action, looks stable), but allow movement both ways
            if (a.Type == CombatActionType.Parry)
            {
                return new LocomotionConstraints(
                    hasFacingLock: true,
                    facingLock: locked,
                    forbidMoveAgainstFacing: false,
                    moveSpeedMul: 1f,
                    accelMul: 1f,
                    decelMul: 1f);
            }

            // Dodge / Dash: lock facing to direction, movement is applied by CombatDodgeMovementPrePhysicsSystem anyway
            if (a.Type == CombatActionType.Dodge || a.Type == CombatActionType.DodgeDash)
            {
                return new LocomotionConstraints(
                    hasFacingLock: true,
                    facingLock: locked,
                    forbidMoveAgainstFacing: false,
                    moveSpeedMul: 1f,
                    accelMul: 1f,
                    decelMul: 1f);
            }

            return LocomotionConstraints.None;
        }
    }
}