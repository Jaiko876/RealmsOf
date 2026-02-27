using Riftborne.Core.Gameplay.Combat.Model;

namespace Riftborne.Core.Gameplay.Combat.Rules.Abstractions
{
    public interface IAttackInputRules
    {
        AttackInputStep Step(in AttackInputStepRequest r);
    }
}