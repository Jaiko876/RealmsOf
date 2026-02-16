using Riftborne.Core.Abstractions;
using Riftborne.Core.Random;

namespace Riftborne.Core.Factory
{
    public sealed class XorShiftRandomFactory : IRandomFactory
    {
        public IRandomSource Create(uint seed)
        {
            return new XorShiftRandomSource(seed);
        }
    }
}
