using Riftborne.Core.Commands;
using Riftborne.Core.Model.Animation;
using Riftborne.Core.Stores;

namespace Riftborne.Core.Input
{
    public sealed class ActionInputCommandHandler : ICommandHandler<InputCommand>
    {
        private readonly IActionIntentStore _actions;

        public ActionInputCommandHandler(IActionIntentStore actions)
        {
            _actions = actions;
        }

        public void Handle(InputCommand command)
        {
            var light = (command.Buttons & InputButtons.AttackPressed) != 0;
            var heavy = (command.Buttons & InputButtons.AttackHeavyPressed) != 0;

            if (light)
                _actions.Set(command.EntityId, ActionState.LightAttack);
            else if (heavy)
                _actions.Set(command.EntityId, ActionState.HeavyAttack);
        }

    }
}