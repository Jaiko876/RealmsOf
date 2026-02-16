namespace Riftborne.Core.Combat.Rules
{
    public enum CombatRuleId
    {
        // Allow/deny
        AllowParryVsHeavy,
        AllowDodgeVsLight,

        // Windows
        ParryWindowTicks,
        DodgeIFramesTicks,
        DashIFramesTicks,

        // Heavy windup
        HeavyWindupMinTicks,
        HeavyWindupMaxTicks,

        // Costs
        ParryStaminaCost,
        DodgeStaminaCost,
        DashStaminaCost,
        BlockStaminaCostPerSec,

        // Penalties / rewards
        ParryFailVsHeavy_ExtraStaminaPenalty,
        DodgeFailVsLight_ExtraStaggerPenalty,
        ParrySuccess_StaggerToAttacker,
        DodgeSuccess_StaminaDamageToAttacker,
        DodgeSuccess_MicroStaggerToAttackerTicks,

        // Behavior flags
        AttackHitsOncePerAction
    }
}
