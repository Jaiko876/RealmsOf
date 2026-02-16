using System;

namespace Riftborne.Core.Model
{
    public readonly struct GameEntityId : IEquatable<GameEntityId>
    {
        public readonly int Value;
        public GameEntityId(int value) { Value = value; }
        public override string ToString() => Value.ToString();

        public bool Equals(GameEntityId other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is GameEntityId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}
