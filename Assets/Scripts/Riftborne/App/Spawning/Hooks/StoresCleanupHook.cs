using Riftborne.App.Spawning.Hooks.Lifecycle;
using Riftborne.Core.Model;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.App.Spawning.Hooks
{
    public sealed class StoresCleanupHook : EntityLifecycleHookBase
    {
        private readonly IMotorStateStore _motorState;
        private readonly IAttackChargeStore _charge;
        private readonly IStatsStore _stats;
        private readonly IStatsEffectStore _effects;
        private readonly IActionEventStore _actionEventStore;
        private readonly IAttackHoldStore _holdStore;
        private readonly IEquippedWeaponStore _equippedWeaponStore;
        private readonly ICombatActionStore _combatActions;

        public StoresCleanupHook(
            IMotorStateStore motorState,
            IAttackChargeStore charge,
            IStatsStore stats,
            IStatsEffectStore effects, 
            IActionEventStore actionEventStore,
            IAttackHoldStore holdStore, 
            IEquippedWeaponStore equippedWeaponStore, 
            ICombatActionStore combatActions)
        {
            _motorState = motorState;
            _charge = charge;
            _stats = stats;
            _effects = effects;
            _actionEventStore = actionEventStore;
            _holdStore = holdStore;
            _equippedWeaponStore = equippedWeaponStore;
            _combatActions = combatActions;
        }

        public override void OnAfterDespawn(GameEntityId id)
        {
            _motorState.Remove(id);
            _charge.Remove(id);
            _effects.ClearEntity(id);
            _stats.Remove(id);
            _actionEventStore.Remove(id);
            _holdStore.Remove(id);
            _equippedWeaponStore.Remove(id);
            _combatActions.Remove(id);
        }
    }
}