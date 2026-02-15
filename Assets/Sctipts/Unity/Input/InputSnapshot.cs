namespace Game.Unity.Input
{
    public struct InputSnapshot
    {
        public float MoveX;
        public float MoveY;

        // Platformer-friendly: held + edge
        public bool JumpHeld;
        public bool JumpPressed;


        public bool HasSomething =>
            MoveX != 0f || MoveY != 0f || JumpHeld || JumpPressed;
    }
}
