using UnityEngine;

namespace BubbleShooter
{
    public class App : MonoBehaviour
    {
        [SerializeField] private SoundManager _soundManager;
        [SerializeField] private Game _game;

        private AppSaveData _appData;

        private void Awake()
        {
            LoadData();
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
    }
}