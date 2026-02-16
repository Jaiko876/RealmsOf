using Riftborne.Core.Abstractions;
using Riftborne.Core.Combat.Abilities;

namespace Riftborne.App.Combat
{
    public sealed class UseAbilityCommandHandler 
        : ICommandHandler<UseAbilityCommand>
    {
        private readonly IAbilitySystem _abilitySystem;

        public UseAbilityCommandHandler(IAbilitySystem abilitySystem)
        {
            _abilitySystem = abilitySystem;
        }

        public void Handle(UseAbilityCommand command)
        {
            _abilitySystem.Use(
                command.Tick,
                command.EntityId,
                command.Slot);
        }
    }
}
