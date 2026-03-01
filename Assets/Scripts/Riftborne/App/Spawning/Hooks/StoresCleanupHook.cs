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
        private readonly ICombatActionCooldownStore _combatCooldowns;
        private readonly IDefenseHoldStore _defenceHold;
        private readonly IBlockStateStore _blockState;

        public StoresCleanupHook(
            IMotorStateStore motorState,
            IAttackChargeStore charge,
            IStatsStore stats,
            IStatsEffectStore effects, 
            IActionEventStore actionEventStore,
            IAttackHoldStore holdStore, 
            IEquippedWeaponStore equippedWeaponStore, 
            ICombatActionStore combatActions, 
            ICombatActionCooldownStore combatCooldowns, 
            IDefenseHoldStore defenceHold, 
            IBlockStateStore blockState)
        {
            _motorState = motorState;
            _charge = charge;
            _stats = stats;
            _effects = effects;
            _actionEventStore = actionEventStore;
            _holdStore = holdStore;
            _equippedWeaponStore = equippedWeaponStore;
            _combatActions = combatActions;
            _combatCooldowns = combatCooldowns;
            _defenceHold = defenceHold;
            _blockState = blockState;
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
            _combatCooldowns.Remove(id);
            _defenceHold.Remove(id);
            _blockState.Remove(id);
        }
    }
}