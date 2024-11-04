using System;
using DG.Tweening;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] private Transform _content;

    private Tween _tween;

    private void OnDestroy()
    {
        _tween?.Kill();
    }

    public void ShowWithAnimation(Action OnComplete = null)
    {
        _content.localScale = Vector3.zero;

        _tween?.Kill();
        _tween = _content
            .DOScale(1f, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => OnComplete?.Invoke());
    }

    public void HideWithAnimation(Action OnComplete = null)
    {
        _content.localScale = Vector3.one;

        _tween?.Kill();
        _tween = _content
            .DOScale(0f, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() => OnComplete?.Invoke());
    }
}
