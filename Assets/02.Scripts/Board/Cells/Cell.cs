using UnityEngine;

namespace KKH.Board
{
    public class Cell : BaseBehaviour
    {
        private Vector2Int _coordinates;
        public Vector2Int Coordinates { get { return _coordinates; } }

        private Tile _tile;
        public Tile Tile { get { return _tile; } }

        public void SetCell(Vector2Int coordinates)
        {
            _coordinates = coordinates;
        }
        public void SetTile(Tile tile)
        {
            _tile = tile;
        }

        public void RemoveTile()
        {
            _tile = null;
        }
    }
}