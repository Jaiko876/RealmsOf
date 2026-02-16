namespace Game.Unity.Input
{
    public struct InputSnapshot
    {
        public float MoveX;
        public float MoveY;

        // Platformer-friendly
        public bool JumpHeld;
        public bool JumpPressed;

        // Combat buttons (edge + held)
        public bool AttackHeld;
        public bool AttackPressed;

        public bool DefenseHeld;
        public bool DefensePressed;

        public bool EvadePressed;

        public bool HasSomething =>
            MoveX != 0f || MoveY != 0f ||
            JumpHeld || JumpPressed ||
            AttackHeld || AttackPressed ||
            DefenseHeld || DefensePressed ||
            EvadePressed;
    }
}
