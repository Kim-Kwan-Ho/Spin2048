using KKH.Manager;
using UnityEngine;
using UnityEngine.UI;



namespace KKH.UI
{
    public class GameOverPanel : BaseBehaviour
    {
        [Header("UI")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _restartButton;

        protected override void Initialize()
        {
            base.Initialize();
            _restartButton.onClick.AddListener(RestartGame);
            _canvas.enabled = false;
        }


        private void OnEnable()
        {
            GameManager.Instance.OnGameOver += ShowGameOver;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnGameOver -= ShowGameOver;
        }
        private void ShowGameOver()
        {
            _canvas.enabled = true;
        }



        private void RestartGame()
        {
            GameManager.Instance.OnGameRestart?.Invoke();
            _canvas.enabled = false;
        }

#if UNITY_EDITOR
        protected override void OnBindField()
        {
            base.OnBindField();
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _restartButton = FindGameObjectInChildren<Button>("RestartButton");
        }

#endif

    }
}