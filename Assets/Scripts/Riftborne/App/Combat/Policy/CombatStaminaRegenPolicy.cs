using System;
using Riftborne.Core.Gameplay.Resources;
using Riftborne.Core.Model;
using Riftborne.Core.Stats;
using Riftborne.Core.Stores.Abstractions;

namespace Riftborne.App.Combat.Policy
{
    public sealed class CombatStaminaRegenPolicy : IResourceRegenPolicy
    {
        private readonly ICombatActionStore _actions;
        private readonly IAttackChargeStore _charge;
        private readonly IBlockStateStore _block;

        public CombatStaminaRegenPolicy(
            ICombatActionStore actions,
            IAttackChargeStore charge,
            IBlockStateStore block)
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _charge = charge ?? throw new ArgumentNullException(nameof(charge));
            _block = block ?? throw new ArgumentNullException(nameof(block));
        }

        public bool CanRegen(GameEntityId id, StatsResource resource, int tick)
        {
            if (resource != StatsResource.Stamina)
                return true;

            if (_block.IsBlocking(id))
                return false;

            if (_charge.TryGet(id, out var charging, out var _) && charging)
                return false;

            if (_actions.TryGet(id, out var a) && a.IsRunningAt(tick))
                return false;

            return true;
        }
    }
}