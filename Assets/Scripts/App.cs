using UnityEngine;

namespace BubbleShooter
{
    public class App : MonoBehaviour
    {
        [SerializeField] private SoundManager _soundManager;
        [SerializeField] private GameUI _gameUI;
        [SerializeField] private Game _game;

        private AppSaveData _appData;

        public int BestScore => _appData.BestScore;

        private void Awake()
        {
            LoadData();
        }

        private void OnEnable()
        {
            _game.Finished += OnGameFinished;
        }

        private void OnDisable()
        {
            _game.Finished -= OnGameFinished;
        }

        private void Start()
        {
            _game.Restart();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                SaveData();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                SaveData();
        }

        public void SaveData()
        {
            _appData.SoundsEnabled = _soundManager.SoundsEnabled;
            _appData.Save();
        }

        public void LoadData()
        {
            _appData = new AppSaveData();
            _appData.Load();

            _soundManager.SoundsEnabled = _appData.SoundsEnabled;
        }

        private void OnGameFinished()
        {
            if (_game.Score > _appData.BestScore)
            {
                _appData.BestScore = _game.Score;
                SaveData();
            }

            _gameUI.ShowResultPopup(_game.Score, _appData.BestScore);
        }
    }
}