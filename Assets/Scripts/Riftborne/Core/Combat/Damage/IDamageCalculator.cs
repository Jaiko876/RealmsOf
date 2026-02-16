using Riftborne.Core.Stats;

namespace Riftborne.Core.Combat.Damage
{
    public interface IDamageCalculator
    {
        DamageResult Calculate(in DamageRequest request);
    }

    public sealed class DefaultDamageCalculator : IDamageCalculator
    {
        private readonly StatResolver _stats;
        private readonly DamageTuning _tuning;

        public DefaultDamageCalculator(StatResolver stats, DamageTuning tuning)
        {
            _stats = stats;
            _tuning = tuning;
        }

        public DamageResult Calculate(in DamageRequest request)
        {
            float hp = CalculateHpDamage(in request);

            // Пока stamina/stagger не считаем (введём следующей порцией)
            return new DamageResult(hp, request.BaseStaminaDamage, request.BaseStaggerBuild);
        }

        private float CalculateHpDamage(in DamageRequest request)
        {
            float baseDamage = request.BaseHpDamage;
            if (baseDamage <= 0f)
                return 0f;

            float dealtMul = _stats.Get(request.Attacker, StatId.DamageDealtMultiplier);
            if (dealtMul <= 0f) dealtMul = 1f;

            float takenMul = _stats.Get(request.Target, StatId.DamageTakenMultiplier);
            if (takenMul <= 0f) takenMul = 1f;

            float defense = _stats.Get(request.Target, StatId.Defense);
            float pen = _stats.Get(request.Attacker, StatId.ArmorPenetration);

            float defEff = defense - pen;
            if (defEff < 0f) defEff = 0f;

            float k = _tuning.DefenseK;
            if (k < 0.0001f) k = 0.0001f;

            float mitigation = defEff / (defEff + k); // 0..1
            float afterDefense = baseDamage * (1f - mitigation);

            // Минимальная доля от baseDamage (анти-“0 урона” при большом defense)
            float minByFraction = baseDamage * _tuning.MinDamageFraction;
            if (afterDefense < minByFraction)
                afterDefense = minByFraction;

            float finalDamage = afterDefense * dealtMul * takenMul;

            if (finalDamage < _tuning.MinFlatDamage)
                finalDamage = _tuning.MinFlatDamage;

            return finalDamage;
        }
    }
}
