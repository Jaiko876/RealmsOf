using Riftborne.Core.Model;
using Riftborne.Core.Stores;

namespace Riftborne.Core.Spawning.Hook
{
    public sealed class StoresCleanupHook : EntityLifecycleHookBase
    {
        private readonly IMotorStateStore _motorState;
        private readonly IActionIntentStore _actions;
        private readonly IActionTimingStore _timings;
        private readonly IAttackChargeStore _charge;
        private readonly IStatsStore _stats;
        private readonly IStatsEffectStore _effects;

        public StoresCleanupHook(
            IMotorStateStore motorState,
            IActionIntentStore actions,
            IActionTimingStore timings,
            IAttackChargeStore charge,
            IStatsStore stats,
            IStatsEffectStore effects)
        {
            _motorState = motorState;
            _actions = actions;
            _timings = timings;
            _charge = charge;
            _stats = stats;
            _effects = effects;
        }

        public override void OnAfterDespawn(GameEntityId id)
        {
            _motorState.Remove(id);
            _actions.Remove(id);
            _timings.Remove(id);
            _charge.Remove(id);
            _effects.ClearEntity(id);
            _stats.Remove(id);
        }
    }
}