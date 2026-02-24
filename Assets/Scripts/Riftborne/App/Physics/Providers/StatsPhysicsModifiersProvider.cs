using System;
using Riftborne.Core.Config;
using Riftborne.Core.Gameplay.Physics.Modifiers;
using Riftborne.Core.Gameplay.Physics.Providers;
using Riftborne.Core.Model;
using Riftborne.Core.Stats;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.App.Physics.Providers
{
    public sealed class StatsPhysicsModifiersProvider : IPhysicsModifiersProvider
    {
        private readonly IStatsStore _stats;
        private readonly IGameplayTuning _tuning;

        public StatsPhysicsModifiersProvider(IStatsStore stats, IGameplayTuning tuning)
        {
            _stats = stats ?? throw new ArgumentNullException(nameof(stats));
            _tuning = tuning ?? throw new ArgumentNullException(nameof(tuning));
        }

        public PhysicsModifiers Get(GameEntityId entityId)
        {
            if (!_stats.TryGet(entityId, out var s) || !s.IsInitialized)
                return PhysicsModifiers.None;

            var sp = _tuning.StatsToPhysics;

            float mul = s.GetEffective(StatId.MoveSpeed);
            mul = Clamp(mul, sp.MinMoveSpeedMultiplier, sp.MaxMoveSpeedMultiplier);

            return new PhysicsModifiers(
                gravityScaleMultiplier: 1f,
                impulseX: 0f,
                impulseY: 0f,
                moveSpeedMultiplier: mul,
                accelMultiplier: mul,
                decelMultiplier: mul);
        }

        private static float Clamp(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }
    }
}