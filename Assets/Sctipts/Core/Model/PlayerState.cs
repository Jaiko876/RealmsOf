namespace Game.Core.Model
{
    public sealed class PlayerState
    {
        public PlayerId Id { get; }

        public float X { get; private set; }
        public float Y { get; private set; }

        public PlayerState(PlayerId id)
        {
            Id = id;
        }

        public void Move(float dx, float dy)
        {
            X += dx;
            Y += dy;
        }
    }
}
