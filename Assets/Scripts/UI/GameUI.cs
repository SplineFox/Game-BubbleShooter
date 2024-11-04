using UnityEngine;

namespace BubbleShooter
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private Game _game;
        [SerializeField] private SoundManager _soundManager;

        [SerializeField] private SoundSwitch _soundSwitch;
        [SerializeField] private ResultPopup _resultPopup;
        [SerializeField] private GameObject _blocker;
        [SerializeField] private DarkBack _darkBack;

        private void Start()
        {
            _soundSwitch.SetState(_soundManager.SoundsEnabled);
            _resultPopup.gameObject.SetActive(false);
            _blocker.SetActive(false);
        }

        public void OnSoundSwitchClicked()
        {
            _soundManager.SoundsEnabled = !_soundManager.SoundsEnabled;
            _soundSwitch.SetState(_soundManager.SoundsEnabled);
        }

        public void OnGameRestartClicked()
        {
            if (_resultPopup.isActiveAndEnabled)
                HideResultPopup();

            _game.Restart();
        }

        public void ShowResultPopup(int score, int bestScore)
        {
            _resultPopup.gameObject.SetActive(true);
            _resultPopup.Setup(score.ToString(), bestScore.ToString());
            _resultPopup.ShowWithAnimation();
            
            _blocker.SetActive(true);

            _darkBack.gameObject.SetActive(true);
            _darkBack.Show(() =>
            {
                _blocker.SetActive(false);
            });
        }

        public void HideResultPopup()
        {
            _blocker.SetActive(true);
            _resultPopup.HideWithAnimation();
            _darkBack.Hide(() =>
            {
                _blocker.SetActive(false);
                _darkBack.gameObject.SetActive(false);
                _resultPopup.gameObject.SetActive(false);
            });
        }
    }
}