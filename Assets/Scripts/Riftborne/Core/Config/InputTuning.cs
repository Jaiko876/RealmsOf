namespace Riftborne.Core.Config
{
    public readonly struct InputTuning
    {
        public readonly float FacingDeadzone;
        public readonly float MoveSpeedDeadzone01;


        public InputTuning(float facingDeadzone, float moveSpeedDeadzone01)
        {
            FacingDeadzone = facingDeadzone;
            MoveSpeedDeadzone01 = moveSpeedDeadzone01;
        }
    }
}