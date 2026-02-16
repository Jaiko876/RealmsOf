using Game.Core.Level;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Unity.Level
{
    [CreateAssetMenu(menuName = "Game/Level/Tileset")]
    public sealed class LevelTilesetAsset : ScriptableObject
    {
        public TileBase GrassTop;
        public TileBase Dirt;
        public TileBase Stone;

        public TileBase Get(LevelTileKind kind)
        {
            switch (kind)
            {
                case LevelTileKind.GrassTop: return GrassTop;
                case LevelTileKind.Dirt: return Dirt;
                case LevelTileKind.Stone: return Stone;
                default: return null;
            }
        }
    }
}
