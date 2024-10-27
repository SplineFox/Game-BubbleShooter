using UnityEngine;
using DG.Tweening;
using TMPro;

namespace BubbleShooter
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField, Min(0f)] private float _changeDuration = 1f;

        private int _currentValue;
        private int _targetValue;

        private Tween _changeTween;

        private int CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                _scoreText.text = value.ToString();
            }
        }

        private void OnDestroy()
        {
            _changeTween?.Kill();
        }

        public void SetValue(int value)
        {
            _targetValue = value;
            PlayChangedAnimation();
        }

        private void PlayChangedAnimation()
        {
            _changeTween?.Kill();
            _changeTween = DOTween.To(() => CurrentValue, x => CurrentValue = x, _targetValue, _changeDuration);
        }
    }
}