using Riftborne.Core.Gameplay.Combat.Model;

namespace Riftborne.Core.Gameplay.Combat.Rules.Abstractions
{
    public interface ICombatRulesEngine
    {
        CombatHitResult Resolve(in CombatResolutionContext ctx);
    }
}