using Game.Core.Model;

namespace Game.Core.Combat.Rules
{
    public interface ICombatRulesResolver
    {
        // allow/deny
        bool AllowParryVsHeavy(GameEntityId defender);
        bool AllowDodgeVsLight(GameEntityId defender);

        // windows
        int GetParryWindowTicks(GameEntityId defender);
        int GetDodgeIFramesTicks(GameEntityId defender);
        int GetDashIFramesTicks(GameEntityId defender);

        // heavy windup variability (attacker-based)
        int GetHeavyWindupMinTicks(GameEntityId attacker);
        int GetHeavyWindupMaxTicks(GameEntityId attacker);

        // costs
        float GetParryStaminaCost(GameEntityId user);
        float GetDodgeStaminaCost(GameEntityId user);
        float GetDashStaminaCost(GameEntityId user);

        // penalties / rewards
        float GetParryFailVsHeavyExtraStaminaPenalty(GameEntityId defender);
        float GetDodgeFailVsLightExtraStaggerPenalty(GameEntityId defender);
        float GetParrySuccessStaggerToAttacker(GameEntityId attacker);
        float GetDodgeSuccessStaminaDamageToAttacker(GameEntityId attacker);
        int GetDodgeSuccessMicroStaggerTicksToAttacker(GameEntityId attacker);

        bool AttackHitsOncePerAction();
    }
}
