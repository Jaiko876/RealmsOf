namespace Riftborne.Core.Combat.Health
{
    public struct HealthState
    {
        public float CurrentHp;
        public bool IsDead;

        // Для regen/дебага
        public int LastDamageTick;

        public HealthState(float currentHp, bool isDead, int lastDamageTick)
        {
            CurrentHp = currentHp;
            IsDead = isDead;
            LastDamageTick = lastDamageTick;
        }
    }
}
