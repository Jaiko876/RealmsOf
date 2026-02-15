namespace Game.Unity.Input
{
    public struct InputSnapshot
    {
        public float MoveX;
        public float MoveY;
        public bool Jump;

        public bool HasMovement =>
            MoveX != 0f || MoveY != 0f;
    }
}
