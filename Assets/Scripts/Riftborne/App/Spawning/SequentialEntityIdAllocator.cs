using Riftborne.App.Spawning.Abstractions;
using Riftborne.Core.Model;

namespace Riftborne.App.Spawning
{
    public sealed class SequentialEntityIdAllocator : IEntityIdAllocator
    {
        private int _next;

        public SequentialEntityIdAllocator(int startFrom)
        {
            _next = startFrom;
        }

        public GameEntityId Next()
        {
            var id = new GameEntityId(_next);
            _next++;
            return id;
        }
    }
}