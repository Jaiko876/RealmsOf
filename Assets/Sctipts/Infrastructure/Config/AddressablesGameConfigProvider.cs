using Cysharp.Threading.Tasks;
using Game.Configs;
using UnityEngine.AddressableAssets;

namespace Game.Infrastructure.Config
{
    public sealed class AddressablesGameConfigProvider : IGameConfigProvider
    {
        private readonly string _key;

        public AddressablesGameConfigProvider(string key)
        {
            _key = key;
        }

        public async UniTask<GameConfigAsset> LoadAsync()
        {
            var handle = Addressables.LoadAssetAsync<GameConfigAsset>(_key);

            // Самый надёжный вариант: ждать через Task
            var asset = await handle.Task;

            return asset;
        }
    }
}
