namespace Riftborne.Core.Model.Animation
{
    public readonly struct ActionEvent
    {
        public ActionState Action { get; }
        public int DurationTicks { get; }

        public ActionEvent(ActionState action, int durationTicks)
        {
            Action = action;
            DurationTicks = durationTicks < 0 ? 0 : durationTicks;
        }

        public bool IsNone => Action == ActionState.None;

        public static ActionEvent None => new ActionEvent(ActionState.None, 0);
    }
}