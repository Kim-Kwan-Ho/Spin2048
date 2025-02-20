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
        private void SpawnTile()
        {
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
            }
        }

        private void Update()
        {
            if (!_canMove) return;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Move(Vector2Int.right, _boardSetting.Row - 2, -1, 0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Move(Vector2Int.left, 1, 1, 0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Move(Vector2Int.up, 0, 1, 1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Move(Vector2Int.down, 0, 1, _boardSetting.Col - 2, -1);
            }
        }

        private void TurnBoard(bool right)
        {

        }

        private void PullDownTiles()
        {

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
            bool moved = false;

            for (int x = startX; x >= 0 && x < _boardSetting.Row; x += incrementX)
            {
                for (int y = startY; y >= 0 && y < _boardSetting.Col; y += incrementY)
                {
                    Cell cell = _cells[x, y];

                    if (cell.Tile != null && MoveTile(cell.Tile, direction))
                    {
                        moved = true;
                    }
                }
            }

            if (moved)
            {
                StartCoroutine(WaitForChanges());
            }
        }

        private bool MoveTile(Tile tile, Vector2Int direction)
        {
            Cell newCell = null;
            Cell adjacent = GetCell(tile.Cell.Coordinates.x + direction.x, tile.Cell.Coordinates.y -direction.y);

            while (adjacent != null)
            {
                if (adjacent.Tile != null)
                {
                    if (tile.CheckMerge(adjacent.Tile))
                    {
                        MergeTiles(tile, adjacent.Tile);
                        return true;
                    }
                    break;
                }

                newCell = adjacent;
                adjacent = GetCell(adjacent.Coordinates.x + direction.x, adjacent.Coordinates.y -direction.y);
            }

            if (newCell != null)
            {
                tile.Move(newCell);
                return true;
            }

            return false;
        }

        private Cell GetCell(int x, int y)
        {
            return (x >= 0 && x < _boardSetting.Row && y >= 0 && y < _boardSetting.Col) ? _cells[x, y] : null;
        }

        private void MergeTiles(Tile a, Tile b)
        {
            _tileList.Remove(a);
            a.Merge(b.Cell);
            b.UpgradeTileStep();
        }

        private IEnumerator WaitForChanges()
        {
            _canMove = false;
            yield return new WaitForSeconds(0.1f);
            _canMove = true;

            foreach (var tile in _tileList)
            {
                tile.UnLockTile();
            }

            if (_tileList.Count < _boardSetting.GetTotalTileCount())
            {
                SpawnTile();
            }
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
