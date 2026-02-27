using System;
using Riftborne.App.Combat.Providers.Abstractions;
using Riftborne.Core.Config;
using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Model;
using Riftborne.Core.Stats;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.App.Combat.Providers
{
    public sealed class StatsCombatSpeedProvider : ICombatSpeedProvider
    {
        private readonly IStatsStore _stats;
        private readonly CombatInputTuning _tuning;

        public StatsCombatSpeedProvider(IStatsStore stats, IGameplayTuning gameplayTuning)
        {
            _stats = stats ?? throw new ArgumentNullException(nameof(stats));
            if (gameplayTuning == null) throw new ArgumentNullException(nameof(gameplayTuning));

            _tuning = gameplayTuning.CombatInput;
        }

        public CombatSpeeds Get(GameEntityId entityId)
            => GetOrDefault(entityId, CombatSpeeds.Default);

        public CombatSpeeds GetOrDefault(GameEntityId entityId, in CombatSpeeds fallback)
        {
            float attack = fallback.AttackSpeed;
            float charge = fallback.ChargeSpeed;

            if (_stats.TryGet(entityId, out var s) && s.IsInitialized)
            {
                attack = s.GetEffective(StatId.AttackSpeed);
                charge = s.GetEffective(StatId.ChargeSpeed);
            }

            attack = Clamp(attack, _tuning.MinAttackSpeed, _tuning.MaxAttackSpeed);
            charge = Clamp(charge, _tuning.MinChargeSpeed, _tuning.MaxChargeSpeed);

            return new CombatSpeeds(attack, charge);
        }

        private static float Clamp(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }
    }
}