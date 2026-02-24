using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;

namespace Riftborne.Core.Stores.Abstractions
{
    public interface IActionTimingStore
    {
        void Set(GameEntityId id, ActionState action, int durationTicks);
        bool TryConsume(GameEntityId id, out ActionState action, out int durationTicks);

        void Remove(GameEntityId id);
        void Clear();
    }
}