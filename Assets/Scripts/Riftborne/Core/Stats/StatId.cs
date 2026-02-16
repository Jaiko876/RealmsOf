namespace Riftborne.Core.Stats
{
    public enum StatId
    {
        // Health (для любого Damageable)
        MaxHp,
        HpRegenPerSec,

        // Damage / Defense baseline (понадобится скоро)
        Defense,
        ArmorPenetration,
        DamageTakenMultiplier,
        DamageDealtMultiplier,

        // Combat resources (только для combatant)
        MaxStamina,
        StaminaRegenPerSec,
        StaminaRegenDelaySec,

        MaxStagger,
        StaggerDecayPerSec,
        StaggerVulnerableBonus,

        // Movement (уже пригодится врагам/персонажам)
        MoveSpeed,
        JumpImpulse,

        // Timing windows (для боевки)
        ParryWindowSec,
        DodgeIFramesSec,
        BlockDamageTakenMultiplier,
        BlockStaminaCostPerSec,

        // Attack base (пер-оружие/пер-атака, но через resolver удобно)
        LightDamageHp,
        LightDamageStamina,
        LightBuildStagger,

        HeavyDamageHp,
        HeavyDamageStamina,
        HeavyBuildStagger
    }
}
