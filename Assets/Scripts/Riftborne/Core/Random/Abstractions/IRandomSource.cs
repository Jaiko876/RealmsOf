namespace Riftborne.Core.Random.Abstractions
{

    public interface IRandomSource
    {
        int NextInt(int minInclusive, int maxExclusive);
        float NextFloat01();
    }
}