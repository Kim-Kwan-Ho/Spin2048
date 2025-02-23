using System;
using UnityEngine;



namespace KKH.Manager
{

    public class ScoreManager : BaseBehaviour
    {
        public static ScoreManager Instance;


        public event Action<int> OnScoreChanged;
        public event Action<int> OnHighScoreChanged;


        private int _highScore;
        private int _currentScore;



        private const string ScoreSavePath = "HighScore";
        protected override void Initialize()
        {
            base.Initialize();
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            InitializeScore();
        }

        private void OnEnable()
        {
            GameManager.Instance.OnGameOver += SaveHighScore;
            GameManager.Instance.OnGameRestart += InitializeScore;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnGameOver -= SaveHighScore;
            GameManager.Instance.OnGameRestart -= InitializeScore;
        }

        public void AddScore(int amount)
        {
            _currentScore += amount;
            OnScoreChanged?.Invoke(_currentScore);

            if (_currentScore > _highScore)
            {
                _highScore = _currentScore;
                OnHighScoreChanged?.Invoke(_highScore);
            }
        }

        private void InitializeScore()
        {
            _currentScore = 0;
            _highScore = GetHighScore();
            OnScoreChanged?.Invoke(_currentScore);
            OnHighScoreChanged?.Invoke(_highScore);
        }


        private void SaveHighScore()
        {
            if (_currentScore >= _highScore && _currentScore > PlayerPrefs.GetInt(ScoreSavePath))
            {
                PlayerPrefs.SetInt(ScoreSavePath, _highScore);
            }
        }

        private int GetHighScore()
        {
            if (PlayerPrefs.HasKey(ScoreSavePath))
            {
                return PlayerPrefs.GetInt(ScoreSavePath);
            }
            else
            {
                PlayerPrefs.SetInt(ScoreSavePath, 0);
                return 0;
            }
        }

    }
}
