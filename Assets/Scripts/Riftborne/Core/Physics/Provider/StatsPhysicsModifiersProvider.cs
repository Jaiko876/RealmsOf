using Riftborne.Core.Model;
using Riftborne.Core.Physics.Abstractions;
using Riftborne.Core.Physics.Model;
using Riftborne.Core.Stores;

namespace Riftborne.Core.Stats
{
    /// <summary>
    /// Bridge: Stats -> PhysicsModifiers (movement).
    /// Keeps Physics independent from Stats internals.
    /// </summary>
    public sealed class StatsPhysicsModifiersProvider : IPhysicsModifiersProvider
    {
        private readonly IStatsStore _stats;

        // Guard rails (so a broken build can't produce absurd physics).
        private const float MinMoveSpeedMul = 0.10f;
        private const float MaxMoveSpeedMul = 3.00f;

        public StatsPhysicsModifiersProvider(IStatsStore stats)
        {
            _stats = stats;
        }

        public PhysicsModifiers Get(GameEntityId entityId)
        {
            if (!_stats.TryGet(entityId, out var s) || !s.IsInitialized)
                return PhysicsModifiers.None;

            float mul = s.GetEffective(StatId.MoveSpeed);
            mul = Clamp(mul, MinMoveSpeedMul, MaxMoveSpeedMul);

            // For now: accel/decel scale together with max speed (feels consistent).
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