using System;
using DG.Tweening;
using UnityEngine;

namespace BubbleShooter
{
    public class DarkBack : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        private Tween _tween;

        public void Show(Action OnComplete)
        {
            _tween?.Kill();
            _tween = _canvasGroup
                .DOFade(1f, 0.5f)
                .OnComplete(() => OnComplete?.Invoke());
        }

        public void Hide(Action OnComplete)
        {
            _tween?.Kill();
            _tween = _canvasGroup
                .DOFade(0f, 0.5f)
                .OnComplete(() => OnComplete?.Invoke());
        }
    }
}