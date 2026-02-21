using Riftborne.Core.Model;

namespace Riftborne.Core.Spawning
{
    public interface IEntityIdAllocator
    {
        GameEntityId Next();
    }
}