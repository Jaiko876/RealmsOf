using Game.Core.Combat.Health;
using Game.Core.Model;
using Game.Core.Stats;

namespace Game.Core.Combat.Damage
{
    public interface IHealthDamageService
    {
        DamageResult Apply(in DamageRequest request, int tick);
    }

    public sealed class HealthDamageService : IHealthDamageService
    {
        private readonly IDamageCalculator _calculator;
        private readonly IHealthStore _health;
        private readonly StatResolver _stats;

        public HealthDamageService(IDamageCalculator calculator, IHealthStore health, StatResolver stats)
        {
            _calculator = calculator;
            _health = health;
            _stats = stats;
        }

        public DamageResult Apply(in DamageRequest request, int tick)
        {
            var result = _calculator.Calculate(in request);

            if (result.FinalHpDamage <= 0f)
                return result;

            float maxHp = _stats.Get(request.Target, StatId.MaxHp);
            if (maxHp <= 0f)
                maxHp = 1f;

            // Если у таргета ещё нет HealthState — создаём с maxHp.
            var hs = _health.GetOrCreate(request.Target, maxHp);

            if (hs.IsDead)
                return result;

            hs.CurrentHp -= result.FinalHpDamage;
            hs.LastDamageTick = tick;

            if (hs.CurrentHp <= 0f)
            {
                hs.CurrentHp = 0f;
                hs.IsDead = true;
            }

            _health.Set(request.Target, hs);

            return result;

        }
    }
}
