using System.Collections.Generic;
using Riftborne.Core.Commands;
using Riftborne.Core.Model;
using Riftborne.Core.Model.Animation;
using Riftborne.Core.Stores;

namespace Riftborne.Core.Input
{
    public sealed class ActionInputCommandHandler : ICommandHandler<InputCommand>
    {
        private readonly IActionIntentStore _actions;
        private readonly IAttackChargeStore _charge;

        // Сколько тиков надо держать, чтобы включить charge.
        // При TickRate=50: 12 тиков ≈ 0.24s
        private const int HeavyThresholdTicks = 21;

        // За сколько тиков после порога набираем Charge01 до 1 (визуально)
        private const int FullChargeExtraTicks = 42;

        private struct Hold
        {
            public bool PrevHeld;
            public int HeldTicks;
        }

        private readonly Dictionary<GameEntityId, Hold> _hold = new Dictionary<GameEntityId, Hold>();

        public ActionInputCommandHandler(IActionIntentStore actions, IAttackChargeStore charge)
        {
            _actions = actions;
            _charge = charge;
        }

        public void Handle(InputCommand command)
        {
            var id = command.EntityId;

            var pressed = (command.Buttons & InputButtons.AttackPressed) != 0;
            var held    = (command.Buttons & InputButtons.AttackHeld) != 0;

            _hold.TryGetValue(id, out var h);

            // начало удержания
            if (pressed)
            {
                h.HeldTicks = 0;
            }

            // тик удержания
            if (held)
            {
                h.HeldTicks++;
            }

            // отпускание (release edge)
            bool released = (!held && h.PrevHeld);

            // вычислим charging + charge01
            bool charging = held && (h.HeldTicks >= HeavyThresholdTicks);
            float charge01 = 0f;

            if (charging)
            {
                int over = h.HeldTicks - HeavyThresholdTicks;
                if (over <= 0) charge01 = 0f;
                else
                {
                    float t = over / (float)FullChargeExtraTicks;
                    if (t < 0f) t = 0f;
                    if (t > 1f) t = 1f;
                    charge01 = t;
                }
            }

            // пишем store (персистентный визуальный флаг)
            _charge.Set(id, charging, charge01);

            // на отпускании решаем, что было: light или heavy
            if (released)
            {
                if (h.HeldTicks >= HeavyThresholdTicks)
                    _actions.Set(id, ActionState.HeavyAttack); // релиз тяжёлой
                else
                    _actions.Set(id, ActionState.LightAttack); // клик

                // сброс
                h.HeldTicks = 0;
                _charge.Set(id, false, 0f);
            }

            h.PrevHeld = held;
            _hold[id] = h;
        }
    }
}
