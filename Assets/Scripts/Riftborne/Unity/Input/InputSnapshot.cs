namespace Riftborne.Unity.Input
{
    public struct InputSnapshot
    {
        public float MoveX;
        public float MoveY;

        public bool JumpHeld;
        public bool JumpPressed;

        public bool AttackHeld;

        public bool DefenseHeld;
        public bool DefensePressed;

        public bool EvadePressed;

        public void ClearEdges()
        {
            JumpPressed = false;
            DefensePressed = false;
            EvadePressed = false;
        }
    }
}