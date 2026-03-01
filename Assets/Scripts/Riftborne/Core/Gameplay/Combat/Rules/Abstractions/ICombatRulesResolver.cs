using Riftborne.Core.Gameplay.Combat.Model;

namespace Riftborne.Core.Gameplay.Combat.Rules.Abstractions
{
    public interface ICombatRulesResolver
    {
        CombatHitResult Resolve(in CombatResolveRequest req);
    }
}