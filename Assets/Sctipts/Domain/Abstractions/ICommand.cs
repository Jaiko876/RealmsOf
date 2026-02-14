namespace Game.Domain.Abstractions
{

    public interface ICommand
    {
        int Tick { get; }
    }
}