using Riftborne.Core.Gameplay.Combat.Model;

namespace Riftborne.Core.Gameplay.Combat.Rules.Abstractions
{
    public interface ICombatHitRules
    {
        CombatHitResult Resolve(in CombatHitContext ctx);
    }

    public readonly struct CombatHitContext
    {
        public readonly CombatActionType Attack;
        public readonly bool DefenderParryActive;
        public readonly bool DefenderDodgeActive;

        public readonly float AttackerAttack;
        public readonly float DefenderDefense;

        public CombatHitContext(
            CombatActionType attack,
            bool defenderParryActive,
            bool defenderDodgeActive,
            float attackerAttack,
            float defenderDefense)
        {
            Attack = attack;
            DefenderParryActive = defenderParryActive;
            DefenderDodgeActive = defenderDodgeActive;
            AttackerAttack = attackerAttack;
            DefenderDefense = defenderDefense;
        }
    }

    public readonly struct CombatHitResult
    {
        // positive numbers mean “amount to apply as damage/consume/build”.
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