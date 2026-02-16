namespace Game.Core.Abstractions
{
    public interface IRandomFactory
    {
        IRandomSource Create(uint seed);
    }
}
