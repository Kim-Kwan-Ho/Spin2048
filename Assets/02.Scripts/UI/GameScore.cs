using KKH.Manager;
using TMPro;
using UnityEngine;

namespace KKH.UI
{
    public class GameScore : BaseBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currentScoreText;
        [SerializeField] private TextMeshProUGUI _highScoreText;
        private void OnEnable()
        {
            ScoreManager.Instance.OnHighScoreChanged += SetHighScoreText;
            ScoreManager.Instance.OnScoreChanged += SetCurrentScoreText;
        }
        private void OnDisable()
        {
            ScoreManager.Instance.OnHighScoreChanged -= SetHighScoreText;
            ScoreManager.Instance.OnScoreChanged -= SetCurrentScoreText;
        }

        private void SetHighScoreText(int score)
        {
            _highScoreText.text = score.ToString();
        }
        private void SetCurrentScoreText(int score)
        {
            _currentScoreText.text = score.ToString();
        }

#if UNITY_EDITOR
        protected override void OnBindField()
        {
            base.OnBindField();
            _highScoreText = FindGameObjectInChildren<TextMeshProUGUI>("HighScoreText");
            _currentScoreText = FindGameObjectInChildren<TextMeshProUGUI>("CurrentScoreText");
        }
#endif
    }

}
