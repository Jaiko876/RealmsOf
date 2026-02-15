namespace Game.Core.Level
{
    public interface ILevelGenPass
    {
        void Apply(LevelGenContext ctx);
    }
}
