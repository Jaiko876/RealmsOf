using Riftborne.Core.Abstractions;

namespace Riftborne.Core.Factory
{
    public interface IRandomFactory
    {
        IRandomSource Create(uint seed);
    }
}
