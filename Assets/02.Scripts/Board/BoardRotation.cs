using System.Collections.Generic;
using UnityEngine;


namespace KKH.Board
{
    public class BoardRotation
    {
        private Dictionary<Vector2Int, Vector2Int> _leftRotationDic;
        public Dictionary<Vector2Int, Vector2Int> LeftRotationDic {  get { return _leftRotationDic; } }
        private Dictionary<Vector2Int, Vector2Int> _rightRotationDic;
        public Dictionary<Vector2Int, Vector2Int> RightRotationDic { get { return _rightRotationDic; } }

        public BoardRotation(int row, int col)
        {
            _leftRotationDic = GenerateLeftRotationMap(col, row);
            _rightRotationDic = GenerateRightRotationMap(col, row);
        }

        private Dictionary<Vector2Int, Vector2Int> GenerateLeftRotationMap(int col, int row)
        {
            var map = new Dictionary<Vector2Int, Vector2Int>();
            for (int i = 0; i < col; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    map[new Vector2Int(row - j - 1, i)] = new Vector2Int(i, j);
                }
            }
            return map;
        }

        private Dictionary<Vector2Int, Vector2Int> GenerateRightRotationMap(int col, int row)
        {
            var map = new Dictionary<Vector2Int, Vector2Int>();
            for (int i = 0; i < col; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    map[new Vector2Int(j, col - i - 1)] = new Vector2Int(i, j);
                }
            }
            return map;
        }

    }
}
