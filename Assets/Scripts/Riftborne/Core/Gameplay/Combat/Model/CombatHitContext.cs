namespace Riftborne.Core.Gameplay.Combat.Model
{
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
}