using Riftborne.Core.Random;

namespace Riftborne.Core.Factory
{
    public interface IRandomFactory
    {
        IRandomSource Create(uint seed);
    }
}
