using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;

namespace Riftborne.Core.Stores.Abstractions
{
    public interface IActionIntentStore
    {
        void Set(GameEntityId id, ActionState action);
        bool TryConsume(GameEntityId id, out ActionState action);
        void Remove(GameEntityId id);
        void Clear();
    }
}