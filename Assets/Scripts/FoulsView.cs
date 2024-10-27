using UnityEngine;
using TMPro;

namespace BubbleShooter
{
    public class FoulsView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;

        private int _currentValue;

        private int CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                _scoreText.text = value.ToString();
            }
        }

        public void SetValue(int value)
        {
            CurrentValue = value;
        }
    }
}