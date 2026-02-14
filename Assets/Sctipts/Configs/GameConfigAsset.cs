using UnityEngine;

namespace Game.Configs {

    [CreateAssetMenu(menuName = "Game/Config/GameConfig")]
    public sealed class GameConfigAsset : ScriptableObject
    {
        [Min(1)] public int TickRate = 50;
        public uint Seed = 12345;
        public float UnitsPerTick = 0.10f;
    }
}
