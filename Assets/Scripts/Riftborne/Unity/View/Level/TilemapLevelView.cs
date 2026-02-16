using Riftborne.App.Level;
using Riftborne.Core.Level;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Riftborne.Unity.View.Level
{
    public sealed class TilemapLevelView : MonoBehaviour, ILevelView
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private LevelTilesetAsset _tileset;

        public void Build(LevelDefinition definition)
        {
            _tilemap.ClearAllTiles();

            // Теперь рисуем не “SolidCells”, а именно Tiles с Kind
            var tiles = definition.Tiles;
            for (int i = 0; i < tiles.Count; i++)
            {
                var t = tiles[i];
                var tileBase = _tileset != null ? _tileset.Get(t.Kind) : null;
                if (tileBase == null) continue;

                _tilemap.SetTile(new Vector3Int(t.X, t.Y, 0), tileBase);
            }
        }
    }
}
