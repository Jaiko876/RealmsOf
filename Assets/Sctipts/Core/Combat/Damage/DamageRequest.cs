using Game.Core.Model;

namespace Game.Core.Combat.Damage
{
    public readonly struct DamageRequest
    {
        public readonly GameEntityId Attacker;
        public readonly GameEntityId Target;

        // Базовые величины “до формул”
        public readonly float BaseHpDamage;

        // На будущее (пока нули)
        public readonly float BaseStaminaDamage;
        public readonly float BaseStaggerBuild;

        public DamageRequest(GameEntityId attacker, GameEntityId target, float baseHpDamage, float baseStaminaDamage, float baseStaggerBuild)
        {
            Attacker = attacker;
            Target = target;
            BaseHpDamage = baseHpDamage;
            BaseStaminaDamage = baseStaminaDamage;
            BaseStaggerBuild = baseStaggerBuild;
        }
    }
}
