using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;

namespace Riftborne.App.Combat.Abstractions
{
    public interface ICombatActionStarter
    {
        void TryStartAttack(GameEntityId id, int tick, ActionState action, int totalDurationTicks, int cooldownTicks, int facing);
        void TryStartParry(GameEntityId id, int tick, int facing);
        void TryStartDodge(GameEntityId id, int tick, int direction);
    }
}