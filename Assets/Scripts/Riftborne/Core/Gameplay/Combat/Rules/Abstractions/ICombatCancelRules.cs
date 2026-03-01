using Riftborne.Core.Gameplay.Combat.Model;

namespace Riftborne.Core.Gameplay.Combat.Rules.Abstractions
{
    public interface ICombatCancelRules
    {
        bool CanCancel(in CombatActionInstance current, CombatActionType next, int tick);
    }
}