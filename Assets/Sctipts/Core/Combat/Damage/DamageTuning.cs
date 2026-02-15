namespace Game.Core.Combat.Damage
{
    public sealed class DamageTuning
    {
        /// <summary>
        /// Масштаб защиты в формуле: mitigation = def / (def + K)
        /// </summary>
        public float DefenseK = 25f;

        /// <summary>
        /// Гарантированный минимум урона как доля base (например 0.05 = 5%),
        /// чтобы defense никогда не превращал урон в “почти ноль”.
        /// </summary>
        public float MinDamageFraction = 0.05f;

        /// <summary>
        /// Минимальный финальный урон (после всего), чтобы вообще не было 0 при ударах,
        /// если ты хочешь “всегда хотя бы 1”.
        /// </summary>
        public float MinFlatDamage = 0f;
    }
}
