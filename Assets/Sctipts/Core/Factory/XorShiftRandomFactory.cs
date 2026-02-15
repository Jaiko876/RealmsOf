using Game.Core.Abstractions;

namespace Game.Core.Random
{
    public sealed class XorShiftRandomFactory : IRandomFactory
    {
        public IRandomSource Create(uint seed)
        {
            return new XorShiftRandomSource(seed);
        }
    }
}
