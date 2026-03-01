using Riftborne.Core.Gameplay.Combat.Model;

namespace Riftborne.Core.Gameplay.Combat.Rules.Abstractions
{
    public interface ICombatRulesModifier
    {
        int Order { get; }
        void Apply(ref CombatResolutionContext ctx);
    }
}