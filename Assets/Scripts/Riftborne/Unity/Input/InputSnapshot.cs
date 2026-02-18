namespace Riftborne.Unity.Input
{
    public struct InputSnapshot
    {
        public float MoveX;
        public float MoveY;

        public bool JumpHeld;
        public bool JumpPressed;

        public bool AttackHeld;
        public bool AttackPressed;
        public bool AttackHeavyPressed;

        public bool DefenseHeld;
        public bool DefensePressed;

        public bool EvadePressed;

        public bool HasSomething =>
            MoveX != 0f || MoveY != 0f ||
            JumpHeld || JumpPressed ||
            AttackHeld || AttackPressed ||
            DefenseHeld || DefensePressed ||
            EvadePressed || AttackHeavyPressed;
    }
}
