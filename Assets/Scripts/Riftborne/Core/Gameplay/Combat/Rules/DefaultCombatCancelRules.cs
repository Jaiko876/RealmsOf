using System;
using Riftborne.Core.Config;
using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Gameplay.Combat.Rules.Abstractions;

namespace Riftborne.Core.Gameplay.Combat.Rules
{
    public sealed class DefaultCombatCancelRules : ICombatCancelRules
    {
        private readonly CombatActionsTuning _t;

        public DefaultCombatCancelRules(IGameplayTuning tuning)
        {
            if (tuning == null) throw new ArgumentNullException(nameof(tuning));
            _t = tuning.CombatActions;
        }

        public bool CanCancel(in CombatActionInstance current, CombatActionType next, int tick)
        {
            if (next != CombatActionType.Dodge)
                return false;

            if (current.Type != CombatActionType.LightAttack && current.Type != CombatActionType.HeavyAttack)
                return false;

            if (!current.IsRunningAt(tick))
                return false;

            var phase = current.GetPhaseAt(tick);
            if (phase != CombatPhase.Recovery)
                return false;

            if (current.Type == CombatActionType.LightAttack)
            {
                // start of recovery is enough
                return true;
            }

            // Heavy: late recovery window
            int elapsed = tick - current.StartTick;
            int recoveryStart = current.WindupTicks + current.ActiveTicks;

            int recoveryTicks = current.RecoveryTicks;
            int offset = (int)MathF.Round(recoveryTicks * _t.Cancel.HeavyDodgeCancelRecoveryStart01);

            return elapsed >= (recoveryStart + offset);
        }
    }
}