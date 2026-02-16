namespace Riftborne.Core.Random
{

    public sealed class XorShiftRandomSource : IRandomSource
    {
        private uint _state;

        public XorShiftRandomSource(uint seed) => _state = seed != 0 ? seed : 2463534242u;

        public int NextInt(int minInclusive, int maxExclusive)
        {
            var range = maxExclusive - minInclusive;
            if (range <= 0)
                return minInclusive;

            var urange = (uint)range;
            return (int)(NextU32() % urange) + minInclusive;
        }

        public float NextFloat01() => (NextU32() & 0x00FFFFFF) / (float)0x01000000;

        private uint NextU32()
        {
            uint x = _state;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            _state = x;
            return x;
        }
    }
}