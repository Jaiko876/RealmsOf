using Riftborne.Core.Model;

namespace Riftborne.App.Spawning.Abstractions
{
    public interface IEntityIdAllocator
    {
        GameEntityId Next();
    }
}