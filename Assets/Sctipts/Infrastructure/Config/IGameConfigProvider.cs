using Game.Configs;
using Cysharp.Threading.Tasks;

namespace Game.Infrastructure.Config
{

    public interface IGameConfigProvider
    {
        UniTask<GameConfigAsset> LoadAsync();
    }
}