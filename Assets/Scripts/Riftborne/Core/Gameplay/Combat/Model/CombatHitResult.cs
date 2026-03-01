namespace Riftborne.Core.Gameplay.Combat.Model
{
    public readonly struct CombatHitResult
    {
        public readonly int DefenderHpDamage;
        public readonly int DefenderStaminaDamage;
        public readonly int DefenderStaggerBuild;

        public readonly int AttackerStaminaDamage;
        public readonly int AttackerStaggerBuild;

        public CombatHitResult(
            int defenderHpDamage,
            int defenderStaminaDamage,
            int defenderStaggerBuild,
            int attackerStaminaDamage,
            int attackerStaggerBuild)
        {
            DefenderHpDamage = defenderHpDamage < 0 ? 0 : defenderHpDamage;
            DefenderStaminaDamage = defenderStaminaDamage < 0 ? 0 : defenderStaminaDamage;
            DefenderStaggerBuild = defenderStaggerBuild < 0 ? 0 : defenderStaggerBuild;

            AttackerStaminaDamage = attackerStaminaDamage < 0 ? 0 : attackerStaminaDamage;
            AttackerStaggerBuild = attackerStaggerBuild < 0 ? 0 : attackerStaggerBuild;
        }

        public static CombatHitResult None => new CombatHitResult(0, 0, 0, 0, 0);
    }
}