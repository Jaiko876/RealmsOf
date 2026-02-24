namespace Riftborne.Core.Random.Abstractions
{
    public interface IRandomFactory
    {
        IRandomSource Create(uint seed);
    }
}
