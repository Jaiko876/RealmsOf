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
            // V1: пока без “hold-to-heavy” тайминга — просто:
            // - AttackPressed => Light
            // - AttackHeld    => Heavy (если хочешь, потом заменим на threshold)
            var pressed = (command.Buttons & InputButtons.AttackPressed) != 0;
            var held    = (command.Buttons & InputButtons.AttackHeld) != 0;

            if (pressed)
                _actions.Set(command.EntityId, ActionState.LightAttack);
            else if (held)
                _actions.Set(command.EntityId, ActionState.HeavyAttack);
        }
    }
}