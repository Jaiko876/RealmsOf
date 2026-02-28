using Riftborne.Core.Model.Animation;

namespace Riftborne.Core.Gameplay.Weapons.Model
{
    public readonly struct WeaponDefinition
    {
        public readonly WeaponId Id;

        public readonly HitProfile LightHit;
        public readonly HitProfile HeavyHit;

        public WeaponDefinition(WeaponId id, HitProfile lightHit, HitProfile heavyHit)
        {
            Id = id;
            LightHit = lightHit;
            HeavyHit = heavyHit;
        }

        public HitProfile GetHit(ActionState action)
        {
            return action == ActionState.HeavyAttack ? HeavyHit : LightHit;
        }
    }
}