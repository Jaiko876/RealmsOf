using Riftborne.Core.Model;

namespace Riftborne.Core.Gameplay.Combat.Abstractions
{
    public interface ICombatHitQuery
    {
        // Returns count written to results (unique ids not guaranteed, caller may dedupe).
        int OverlapBox(float centerX, float centerY, float width, float height, int layerMask, GameEntityId[] results);
    }
}