using System.Collections;
using TMPro;
using UnityEngine;


namespace KKH.Board
{
    public class Tile : BaseBehaviour
    {

        [SerializeField] private TileSettingSo _tileSettingSo;
        private Cell _cell;
        public Cell Cell { get { return _cell; } }

        [SerializeField] private int _step;
        public int Step { get { return _step; } }

        private bool _isLocked = false;
        public bool IsLocked { get { return _isLocked; } }


        [Header("Visual")]
        [SerializeField] private SpriteRenderer _bgSprite;
        [SerializeField] private TextMeshPro _valueText;


        public void SetTileInfo(int step)
        {
            _step = step;
            _valueText.text = _tileSettingSo.GetValue(_step).ToString();
            _bgSprite.color = _tileSettingSo.GetBgColor(_step);
            _valueText.color = _tileSettingSo.GetTextColor(_step);
        }

        public void SpawnTile(Cell cell, int step)
        {
            ChangeCell(cell);
            SetTileInfo(step);
            transform.position = cell.transform.position;
        }
        public void ChangeCell(Cell cell)
        {
            _cell?.RemoveTile();
            _cell = cell;
            if (cell == null)
            {
                return;
            }
            _cell.SetTile(this);

        }
        public int UpgradeTileStep()
        {
            SetTileInfo(++_step);
            return _tileSettingSo.GetValue(_step-1);
        }

        public bool CheckMerge(Tile tile)
        {
            return _step == tile._step && !tile.IsLocked;
        }
        public void Merge(Cell cell)
        {
            _cell?.RemoveTile();
            cell.Tile.LockTile();
            StartCoroutine(CoMoveTo(cell.transform.position, true));
        }

        public void LockTile()
        {
            _isLocked = true;
        }

        public void UnLockTile()
        {
            _isLocked = false;
        }
        public void Move(Cell cell)
        {
            ChangeCell(cell);
            StartCoroutine(CoMoveTo(cell.transform.position, false));
        }
        private IEnumerator CoMoveTo(Vector2 targetPos, bool merging)
        {
            float time = 0;

            Vector2 startPos = transform.position;

            while (time < _tileSettingSo.MoveDuration)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, time / _tileSettingSo.MoveDuration);
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPos;

            if (merging)
            {
                Destroy(this.gameObject);
            }
        }

        public void DestroyTile()
        {
            _cell?.RemoveTile();
            Destroy(this.gameObject);
        }

#if UNITY_EDITOR
        protected override void OnBindField()
        {
            base.OnBindField();
            _tileSettingSo = FindObjectInAsset<TileSettingSo>();
            _bgSprite = GetComponent<SpriteRenderer>();
            _valueText = FindGameObjectInChildren<TextMeshPro>("ValueText");

        }
#endif
    }


}
