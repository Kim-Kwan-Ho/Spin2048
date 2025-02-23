using UnityEngine;
using UnityEngine.UI;

namespace KKH.UI
{
    public class BoardController : BaseBehaviour
    {
        [Header("Board")]
        [SerializeField] private Board.Board _board;

        [Header("Buttons")]
        [SerializeField] private Button _leftTurnButton;
        [SerializeField] private Button _rightTurnButton;
        [SerializeField] private Button _spawnTileButton;



        protected override void Initialize()
        {
            base.Initialize();
            _leftTurnButton.onClick.AddListener(() => _board.TurnBoard(true));
            _rightTurnButton.onClick.AddListener(() => _board.TurnBoard(false));
            _spawnTileButton.onClick.AddListener(_board.SpawnTile);
        }

#if UNITY_EDITOR
        protected override void OnBindField()
        {
            base.OnBindField();
            _leftTurnButton = FindGameObjectInChildren<Button>("LeftTurnButton");
            _rightTurnButton = FindGameObjectInChildren<Button>("RightTurnButton");
            _spawnTileButton = FindGameObjectInChildren<Button>("SpawnTileButton");
            _board = FindObjectOfType<Board.Board>();
        }
#endif
    }
}
