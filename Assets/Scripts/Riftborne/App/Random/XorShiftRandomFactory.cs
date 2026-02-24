using Riftborne.Core.Random;
using Riftborne.Core.Random.Abstractions;

namespace Riftborne.App.Random
{
    public sealed class XorShiftRandomFactory : IRandomFactory
    {
        public IRandomSource Create(uint seed)
        {
            return new XorShiftRandomSource(seed);
        }
    }
}
