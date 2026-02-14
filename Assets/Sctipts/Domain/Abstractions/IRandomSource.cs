namespace Game.Domain.Abstractions
{

    public interface IRandomSource
    {
        int NextInt(int minInclusive, int maxInclusive);
        float NextFloat01();
    }
}