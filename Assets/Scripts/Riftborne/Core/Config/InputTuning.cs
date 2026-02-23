namespace Riftborne.Core.Config
{
    public readonly struct InputTuning
    {
        public readonly float FacingDeadzone;

        public InputTuning(float facingDeadzone)
        {
            FacingDeadzone = facingDeadzone;
        }
    }
}