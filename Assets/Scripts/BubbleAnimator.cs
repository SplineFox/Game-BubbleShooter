using BubbleShooter;
using BubbleShooter.HexGrids;
using DG.Tweening;
using System;
using UnityEngine;

public class BubbleAnimator : MonoBehaviour
{
    [SerializeField] private HexGridLayout _hexGridLayout;
    [SerializeField] private EffectSpawner _effectSpawner;
    [SerializeField] private float _spawnDuration = 0.4f;

    private Sequence _sequence;

    public void AnimateBubbleMove(Bubble bubble, BubbleTrajectory trajectory, Action onComplete)
    {
        bubble.transform.position = trajectory.FirstPoint.Position;

        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        var isFirst = true;
        foreach (var point in trajectory)
        {
            if (isFirst)
            {
                isFirst = false;
                continue;
            }

            _sequence.Append(bubble.transform.DOMove(point.Position, 0.2f));
        }

        var hexPoint = _hexGridLayout.WorldToHex(trajectory.LastPoint.Position);
        var worldPoint = _hexGridLayout.HexToWorld(hexPoint);

        _sequence.Append(bubble.transform.DOMove(worldPoint, 0.2f));
        _sequence.OnComplete(() => onComplete?.Invoke());
    }

    public void AnimateBubblePop(Bubble bubble)
    {
        var effect = _effectSpawner.Spawn(bubble.transform.position, bubble.TypeId);
        effect.Play();
    }

    public void AnimateBubbleSpawn(Bubble bubble)
    {
        bubble.DOKill();
        bubble.transform.localScale = Vector3.zero;
        bubble.transform.DOScale(Vector3.one, _spawnDuration)
            .SetEase(Ease.OutBack);
    }
}
