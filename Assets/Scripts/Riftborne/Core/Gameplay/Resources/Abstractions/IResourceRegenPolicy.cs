using Riftborne.Core.Model;
using Riftborne.Core.Stats;

namespace Riftborne.Core.Gameplay.Resources
{
    public interface IResourceRegenPolicy
    {
        bool CanRegen(GameEntityId id, StatsResource resource, int tick);
    }

    public sealed class AllowAllResourceRegenPolicy : IResourceRegenPolicy
    {
        public bool CanRegen(GameEntityId id, StatsResource resource, int tick) => true;
    }
}