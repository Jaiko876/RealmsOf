namespace Game.Core.Abstractions
{

    public interface ICommand
    {
        int Tick { get; }
    }
}