using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



namespace KKH.Board
{
    public class Board : BaseBehaviour
    {
        [Header("Board Setting")]
        [SerializeField] private BoardSettingSo _boardSetting;
        private BoardRotation _boardRotation;


        [Header("Cells")]
        [SerializeField] private GameObject _cellPrefab;
        private Cell[,] _cells;
        public Cell[,] Cells { get { return _cells; } }


        [Header("Tiles")]
        [SerializeField] private GameObject _tilePrefab;
        private List<Tile> _tileList;


        private bool _canMove;
        protected override void Initialize()
        {
            base.Initialize();


            _boardRotation = new BoardRotation(_boardSetting.Col, _boardSetting.Row);
            _cells = new Cell[_boardSetting.Col, _boardSetting.Row];
            _tileList = new List<Tile>(_boardSetting.GetTotalTileCount());


            CreateNewBoard();
        }

        private void CreateNewBoard()
        {
            _tileList.Clear();
            float x = _boardSetting.CalcXStartPos();
            float y = _boardSetting.CalcYStartPos();

            for (int r = 0; r < _boardSetting.Row; r++)
            {
                for (int c = 0; c < _boardSetting.Col; c++)
                {
                    _cells[r, c] = Instantiate(_cellPrefab, new Vector2(x + c, y + r), Quaternion.identity).GetComponent<Cell>();
                    _cells[r, c].SetCell(new Vector2Int(r, c));
                }
            }
            _canMove = true;
            SpawnTile();
        }

        public void ResetBoard()
        {
            foreach (var tile in _tileList)
            {
                tile.DestroyTile();
            }
            _tileList.Clear();
            SpawnTile();
            _canMove = true;
        }
        public void SpawnTile()
        {
            if (!_canMove)
                return;

            Cell cell = GetRandomEmptyCell();

            if (cell == null)
            {
                return;
            }
            else
            {
                Tile tile = Instantiate(_tilePrefab, transform).GetComponent<Tile>();
                tile.SpawnTile(cell, _boardSetting.GetSpawnTileLevel());
                _tileList.Add(tile);

                if (_tileList.Count >= _boardSetting.GetTotalTileCount() && CheckGameOver())
                {
                    _canMove = false;
                }
            }
        }

        public void TurnBoard(bool left)
        {
            if (!_canMove)
                return;

            Dictionary<Tile, Vector2Int> newPositions = new Dictionary<Tile, Vector2Int>();

            foreach (var tile in _tileList)
            {
                var pos = left ? _boardRotation.RightRotationDic[tile.Cell.Coordinates] : _boardRotation.LeftRotationDic[tile.Cell.Coordinates];
                newPositions[tile] = pos;
                tile.ChangeCell(null);
            }

            foreach (var tile in _tileList)
            {
                var newPos = newPositions[tile];
                tile.Move(_cells[newPos.x, newPos.y]);
            }
            StartCoroutine(CoPullDownBoardDelay());
        }

        private IEnumerator CoPullDownBoardDelay()
        {
            _canMove = false;
            yield return new WaitForSeconds(_boardSetting.RotateTime);
            PullDownTiles();
        }
        private void PullDownTiles()
        {
            Move(Vector2Int.left, 1, 1, 0, 1);
        }



        private Cell GetRandomEmptyCell()
        {
            HashSet<Vector2Int> hash = new HashSet<Vector2Int>();
            while (true)
            {
                int x = Random.Range(0, _boardSetting.Row);
                int y = Random.Range(0, _boardSetting.Col);

                if (hash.Contains(new Vector2Int(x, y)))
                    continue;

                if (_cells[x, y].Tile == null)
                    return _cells[x, y];

                hash.Add(new Vector2Int(x, y));
                if (hash.Count >= _boardSetting.GetTotalTileCount())
                {
                    Debug.LogError("Tile is all filled");
                    return null;
                }
            }
        }

        private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
        {

            for (int x = startX; x >= 0 && x < _boardSetting.Row; x += incrementX)
            {
                for (int y = startY; y >= 0 && y < _boardSetting.Col; y += incrementY)
                {
                    Cell cell = _cells[x, y];
                    if (cell.Tile != null)
                    {
                        MoveTile(cell.Tile, direction);
                    }
                }
            }

            StartCoroutine(CoWaitForChanges());
        }


        private void MoveTile(Tile tile, Vector2Int direction)
        {
            Cell newCell = null;
            Cell adjacent = GetNextCell(tile.Cell, direction);

            while (adjacent != null)
            {
                if (adjacent.Tile != null)
                {
                    if (tile.CheckMerge(adjacent.Tile))
                    {
                        MergeTiles(tile, adjacent.Tile);
                        return;
                    }
                    break;
                }

                newCell = adjacent;
                adjacent = GetNextCell(adjacent, direction);
            }

            if (newCell != null)
            {
                tile.Move(newCell);
            }
        }
        private void MergeTiles(Tile a, Tile b)
        {
            _tileList.Remove(a);
            a.Merge(b.Cell);
            b.UpgradeTileStep();
        }

        private IEnumerator CoWaitForChanges()
        {
            yield return new WaitForSeconds(_boardSetting.DelayTime);
            foreach (var tile in _tileList)
            {
                tile.UnLockTile();
            }
            if (_tileList.Count >= _boardSetting.GetTotalTileCount())
            {
                Debug.Log("GG");
            }
            else
            {
                _canMove = true;
                SpawnTile();
            }
        }
        private Cell GetNextCell(Cell cell, Vector2Int direction)
        {
            return GetCell(cell.Coordinates.x + direction.x, cell.Coordinates.y - direction.y);
        }
        private Cell GetCell(int x, int y)
        {
            return (x >= 0 && x < _boardSetting.Row && y >= 0 && y < _boardSetting.Col) ? _cells[x, y] : null;
        }




        private bool CheckGameOver()
        {
            Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
            foreach (var tile in _tileList)
            {
                foreach (var direction in directions)
                {
                    Cell adjacentCell = GetNextCell(tile.Cell, direction);
                    if (adjacentCell != null && tile.CheckMerge(adjacentCell.Tile))
                    {
                        return false;
                    }
                }
            }
            Debug.Log("GG");
            return true;
        }


#if UNITY_EDITOR
        protected override void OnBindField()
        {
            base.OnBindField();
            _boardSetting = FindObjectInAsset<BoardSettingSo>();
            _cellPrefab = FindObjectInAsset<Cell>().gameObject;
            _tilePrefab = FindObjectInAsset<Tile>().gameObject;
        }

        protected override void OnButtonField()
        {
            SpawnTile();
        }
#endif
    }
}
