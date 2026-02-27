using Riftborne.Core.Model.Animation;

namespace Riftborne.Core.Gameplay.Combat.Model
{
    public readonly struct ReleaseDecision
    {
        public readonly bool HasRelease;
        public readonly ActionState Action; // Light/Heavy
        public readonly int DurationTicks;
        public readonly int CooldownTicks;

        public ReleaseDecision(bool hasRelease, ActionState action, int durationTicks, int cooldownTicks)
        {
            HasRelease = hasRelease;
            Action = action;
            DurationTicks = durationTicks;
            CooldownTicks = cooldownTicks;
        }

        public static ReleaseDecision None()
        {
            return new ReleaseDecision(false, ActionState.None, 0, 0);
        }
    }
}