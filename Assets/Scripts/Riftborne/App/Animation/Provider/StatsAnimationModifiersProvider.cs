using System;
using Riftborne.Core.Config;
using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;
using Riftborne.Core.Stats;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.App.Animation.Provider
{
    public sealed class StatsAnimationModifiersProvider : IAnimationModifiersProvider
    {
        private readonly IStatsStore _stats;
        private readonly IGameplayTuning _tuning;

        public StatsAnimationModifiersProvider(IStatsStore stats, IGameplayTuning tuning)
        {
            _stats = stats ?? throw new ArgumentNullException(nameof(stats));
            _tuning = tuning ?? throw new ArgumentNullException(nameof(tuning));
        }

        public AnimationModifiers Get(GameEntityId entityId)
        {
            if (!_stats.TryGet(entityId, out var s) || !s.IsInitialized)
                return AnimationModifiers.Default;

            var ci = _tuning.CombatInput;

            float attack = s.GetEffective(StatId.AttackSpeed);
            attack = Clamp(attack, ci.MinAttackSpeed, ci.MaxAttackSpeed);

            float charge = s.GetEffective(StatId.ChargeSpeed);
            charge = Clamp(charge, ci.MinChargeSpeed, ci.MaxChargeSpeed);

            return new AnimationModifiers(attack, charge);
        }

        private static float Clamp(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }
    }
}