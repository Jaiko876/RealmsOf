using Riftborne.Core.Gameplay.Combat.Model;
using Riftborne.Core.Model;

namespace Riftborne.App.Combat.Providers.Abstractions
{
    public interface ICombatSpeedProvider
    {
        CombatSpeeds Get(GameEntityId entityId);

        CombatSpeeds GetOrDefault(GameEntityId entityId, in CombatSpeeds fallback);
    }
}