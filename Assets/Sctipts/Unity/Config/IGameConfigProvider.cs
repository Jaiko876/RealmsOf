using Game.Configs;
using Cysharp.Threading.Tasks;

namespace Game.Unity.Config
{

    public interface IGameConfigProvider
    {
        UniTask<GameConfigAsset> LoadAsync();
    }
}