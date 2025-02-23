using System.Collections;
using System.Collections.Generic;
using KKH.Manager;
using UnityEngine;



namespace KKH.Board
{
    public class Board : BaseBehaviour
    {
        [Header("Board Setting")]
        [SerializeField] private BoardSettingSo _boardSetting;
        private BoardRotation _boardRotation;
       [SerializeField]  private GameObject _boardBackground;

        [Header("Cells")]
        [SerializeField] private GameObject _cellPrefab;
        private Cell[,] _cells;


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


        private void OnEnable()
        {
            GameManager.Instance.OnGameRestart += ResetBoard;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnGameRestart -= ResetBoard;
        }
        private void CreateNewBoard()
        {
            CreateBoardBackground();
            _tileList.Clear();
            float boardWidth = _boardSetting.Col + (_boardSetting.Col - 1) * _boardSetting.CellOffSet;
            float boardHeight = _boardSetting.Row + (_boardSetting.Row - 1) * _boardSetting.CellOffSet;

            // 보드의 중앙 시작 위치 계산
            float xStart = -boardWidth / 2 + 0.5f;
            float yStart = -boardHeight / 2 + 0.5f;

            float offSetY = 0;

            for (int r = 0; r < _boardSetting.Row; r++)
            {
                float offSetX = 0;

                for (int c = 0; c < _boardSetting.Col; c++)
                {
                    _cells[r, c] = Instantiate(
                        _cellPrefab,
                        new Vector2(xStart + c + offSetX, yStart + r + offSetY),
                        Quaternion.identity
                    ).GetComponent<Cell>();

                    _cells[r, c].SetCell(new Vector2Int(r, c));
                    offSetX += _boardSetting.CellOffSet;
                }

                offSetY += _boardSetting.CellOffSet;
            }

            _canMove = true;
            SpawnTile();
        }
        private void CreateBoardBackground()
        {
            GameObject boardBackground = Instantiate(_boardBackground);

            float boardWidth = _boardSetting.Col + (_boardSetting.Col - 1) * _boardSetting.CellOffSet + _boardSetting.BackGroundPadding * 2;
            float boardHeight = _boardSetting.Row + (_boardSetting.Row - 1) * _boardSetting.CellOffSet + _boardSetting.BackGroundPadding * 2;

            boardBackground.transform.position = new Vector2(0,0);
            boardBackground.transform.localScale = new Vector3(boardWidth, boardHeight, 1);
        }
        private void ResetBoard()
        {
            foreach (var tile in _tileList)
            {
                tile.DestroyTile();
            }
            _tileList.Clear();

            _canMove = true;
            SpawnTile();
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
                    GameManager.Instance.OnGameOver?.Invoke();
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
                    return null;
                }
            }
        }

        private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
        {

            int score = 0;
            for (int x = startX; x >= 0 && x < _boardSetting.Row; x += incrementX)
            {
                for (int y = startY; y >= 0 && y < _boardSetting.Col; y += incrementY)
                {
                    Cell cell = _cells[x, y];
                    if (cell.Tile != null)
                    {
                        score += MoveTile(cell.Tile, direction);
                    }
                }
            }

            if (score != 0)
            {
                ScoreManager.Instance.AddScore(score);
            }
            StartCoroutine(CoWaitForChanges());
        }


        private int MoveTile(Tile tile, Vector2Int direction)
        {
            Cell newCell = null;
            Cell adjacent = GetNextCell(tile.Cell, direction);

            while (adjacent != null)
            {
                if (adjacent.Tile != null)
                {
                    if (tile.CheckMerge(adjacent.Tile))
                    {

                        return MergeTiles(tile, adjacent.Tile);
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

            return 0;
        }
        private int MergeTiles(Tile a, Tile b)
        {
            _tileList.Remove(a);
            a.Merge(b.Cell);
            return b.UpgradeTileStep();
        }

        private IEnumerator CoWaitForChanges()
        {
            yield return new WaitForSeconds(_boardSetting.DelayTime);
            foreach (var tile in _tileList)
            {
                tile.UnLockTile();
            }

            yield return new WaitForSeconds(_boardSetting.DelayTime);
            if (_tileList.Count >= _boardSetting.GetTotalTileCount())
            {
                GameManager.Instance.OnGameOver?.Invoke();
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
            return true;
        }


#if UNITY_EDITOR
        protected override void OnBindField()
        {
            base.OnBindField();
            _boardSetting = FindObjectInAsset<BoardSettingSo>();
            _boardBackground = FindObjectInAsset<GameObject>("BoardBackground", EDataType.prefab);
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
