using Game.Core.Level;

namespace Game.App.Level
{
    public interface ILevelView
    {
        void Build(LevelDefinition definition);
    }
}
