using Game.Core.Abstractions;
using Game.Core.Combat.Abilities;

namespace Game.App.Combat
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
