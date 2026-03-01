using Riftborne.Core.Model;

namespace Riftborne.Core.Gameplay.Combat.Model
{
    public readonly struct CombatResolveRequest
    {
        public readonly GameEntityId AttackerId;
        public readonly GameEntityId DefenderId;

        public readonly CombatActionType Attack;

        public readonly bool DefenderParryActive;
        public readonly bool DefenderDodgeActive;
        public readonly bool DefenderBlockActive;

        public readonly float AttackerAttack;
        public readonly float DefenderDefense;

        public CombatResolveRequest(
            GameEntityId attackerId,
            GameEntityId defenderId,
            CombatActionType attack,
            bool defenderParryActive,
            bool defenderDodgeActive,
            bool defenderBlockActive,
            float attackerAttack,
            float defenderDefense)
        {
            AttackerId = attackerId;
            DefenderId = defenderId;
            Attack = attack;

            DefenderParryActive = defenderParryActive;
            DefenderDodgeActive = defenderDodgeActive;
            DefenderBlockActive = defenderBlockActive;

            AttackerAttack = attackerAttack;
            DefenderDefense = defenderDefense;
        }
    }
}