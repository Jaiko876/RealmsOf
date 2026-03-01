// Assets/Scripts/Riftborne/App/Combat/Providers/CombatLocomotionConstraintsProvider.cs
using System;
using Riftborne.Core.Config;
using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Gameplay.Locomotion.Abstractions;
using Riftborne.Core.Model;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.App.Combat.Providers
{
    public sealed class CombatLocomotionConstraintsProvider : ILocomotionConstraintsProvider
    {
        private readonly ICombatActionStore _actions;
        private readonly CombatActionsTuning _tuning;

        public CombatLocomotionConstraintsProvider(ICombatActionStore actions, IGameplayTuning tuning)
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            if (tuning == null) throw new ArgumentNullException(nameof(tuning));
            _tuning = tuning.CombatActions;
        }

        public LocomotionConstraints Get(GameEntityId id, int tick)
        {
            if (!_actions.TryGet(id, out var a))
                return LocomotionConstraints.None;

            if (!a.IsRunningAt(tick))
                return LocomotionConstraints.None;

            // Facing is locked for any combat action (attack/parry/dodge) for its whole lifetime.
            bool lockFacing = a.LockedFacing != 0;
            sbyte facing = a.LockedFacing != 0 ? a.LockedFacing : (sbyte)1;

            // Movement penalty only for attacks during Windup+Active.
            float moveMul = 1f;
            float accelMul = 1f;
            float decelMul = 1f;

            if (a.Type == CombatActionType.LightAttack || a.Type == CombatActionType.HeavyAttack)
            {
                var phase = a.GetPhaseAt(tick);
                if (phase == CombatPhase.Windup || phase == CombatPhase.Active)
                {
                    moveMul = (a.Type == CombatActionType.HeavyAttack)
                        ? _tuning.AttackMovement.HeavyMoveMul
                        : _tuning.AttackMovement.LightMoveMul;

                    accelMul = moveMul;
                    decelMul = moveMul;
                }
            }

            return new LocomotionConstraints(lockFacing, facing, moveMul, accelMul, decelMul);
        }
    }
}