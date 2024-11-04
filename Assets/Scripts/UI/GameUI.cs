using UnityEngine;

namespace BubbleShooter
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private SoundManager _soundManager;
        [SerializeField] private Game _game;

        [SerializeField] private SoundSwitch _soundSwitch;

        private void Start()
        {
            _soundSwitch.SetState(_soundManager.SoundsEnabled);
        }

        public void OnSoundSwitchClicked()
        {
            _soundManager.SoundsEnabled = !_soundManager.SoundsEnabled;
            _soundSwitch.SetState(_soundManager.SoundsEnabled);
        }

        public void OnGameRestartClicked()
        {
            _game.Restart();
        }
    }
}