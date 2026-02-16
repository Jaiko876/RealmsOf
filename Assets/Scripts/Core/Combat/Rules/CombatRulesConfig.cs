namespace Game.Core.Combat.Rules
{
    /// <summary>
    /// Базовые дефолты правил. Это "ваниль" без модификаторов.
    /// Дальше всё может быть переопределено/модифицировано через ICombatRulesModifier.
    /// </summary>
    public sealed class CombatRulesConfig
    {
        // --- Windows / timing (ticks) ---
        public int DefaultParryWindowTicks = 2;
        public int DefaultDodgeIFramesTicks = 3;
        public int DefaultDashIFramesTicks = 0;

        // Heavy windup variability (ticks)
        public int HeavyWindupMinTicks = 6;
        public int HeavyWindupMaxTicks = 10;

        // --- Costs ---
        public float ParryStaminaCost = 1f;
        public float DodgeStaminaCost = 2f;
        public float DashStaminaCost = 1.5f;
        public float BlockStaminaCostPerSec = 2f;

        // --- Wrong reaction penalties ---
        // Attempt Parry vs Heavy: defender loses stamina (in addition to taking heavy hit)
        public float ParryFailVsHeavy_ExtraStaminaPenalty = 2f;

        // Attempt Dodge vs Light: defender gets extra stagger (in addition to taking light hit)
        public float DodgeFailVsLight_ExtraStaggerPenalty = 2.5f;

        // --- Success rewards/punishes ---
        // Parry Light success: attacker gains stagger
        public float ParrySuccess_StaggerToAttacker = 2f;

        // Dodge Heavy success: attacker loses stamina and receives micro-stagger
        public float DodgeSuccess_StaminaDamageToAttacker = 4f;
        public int DodgeSuccess_MicroStaggerToAttackerTicks = 8;

        // --- Allow/deny base (vanilla) ---
        public bool AllowParryVsHeavy = false;
        public bool AllowDodgeVsLight = false;

        // --- One-hit-per-attack (prevents multi-hit each active tick) ---
        public bool AttackHitsOncePerAction = true;
    }
}
