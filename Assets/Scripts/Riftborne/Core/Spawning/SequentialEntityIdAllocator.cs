using Riftborne.Core.Model;

namespace Riftborne.Core.Spawning
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