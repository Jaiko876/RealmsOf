using Riftborne.Core.Model.Animation;

namespace Riftborne.App.Animation.Events
{
    public readonly struct ActionPayload
    {
        public readonly ActionState Action;
        public readonly int ActionTick;
        public readonly int DurationTicks; // 0 = not provided/unknown

        public ActionPayload(ActionState action, int actionTick, int durationTicks)
        {
            Action = action;
            ActionTick = actionTick;
            DurationTicks = durationTicks;
        }

        public static ActionPayload None(int tick) => new ActionPayload(ActionState.None, tick, 0);
    }
}