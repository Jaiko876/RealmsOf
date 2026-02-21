using Riftborne.Core.Model;
using Riftborne.Core.Stores;

namespace Riftborne.Core.Spawning.Hook
{
    public sealed class StoresCleanupHook : EntityLifecycleHookBase
    {
        private readonly IMotorStateStore _motorState;
        private readonly IActionIntentStore _actions;
        private readonly IAttackChargeStore _charge;
        private readonly IStatsStore _stats;
        private readonly IStatsEffectStore _effects;

        public StoresCleanupHook(
            IMotorStateStore motorState,
            IActionIntentStore actions,
            IAttackChargeStore charge,
            IStatsStore stats,
            IStatsEffectStore effects)
        {
            _motorState = motorState;
            _actions = actions;
            _charge = charge;
            _stats = stats;
            _effects = effects;
        }

        public override void OnAfterDespawn(GameEntityId id)
        {
            _motorState.Remove(id);
            _actions.Remove(id);
            _charge.Remove(id);
            _effects.ClearEntity(id);
            _stats.Remove(id);
        }
    }
}