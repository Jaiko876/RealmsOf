using Riftborne.Core.Gameplay.Combat.Model;

namespace Riftborne.Core.Gameplay.Combat.Rules.Abstractions
{
    public interface ICombatHitRules
    {
        CombatHitResult Resolve(in CombatHitContext ctx);
    }
}