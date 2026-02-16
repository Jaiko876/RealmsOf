namespace Game.Core.Model
{
    public readonly struct GameEntityId
    {
        public readonly int Value;
        public GameEntityId(int value) { Value = value; }
        public override string ToString() => Value.ToString();
    }
}
