using Game.App.Level;
using Game.Core.Level;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Unity.Level
{
    public sealed class TilemapLevelView : MonoBehaviour, ILevelView
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private TileBase _groundTile;

        public void Build(LevelDefinition definition)
        {
            _tilemap.ClearAllTiles();

            foreach (var cell in definition.SolidCells)
            {
                _tilemap.SetTile(
                    new Vector3Int(cell.X, cell.Y, 0),
                    _groundTile);
            }
        }
    }
}
