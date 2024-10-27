using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using BubbleShooter;
using BubbleShooter.HexGrids;
using Random = UnityEngine.Random;

public class BubbleAnimator : MonoBehaviour
{
    [SerializeField] private HexGridLayout _hexGridLayout;
    [SerializeField] private float _spawnDuration = 0.4f;
    [SerializeField] private float _moveDuration = 0.2f;
    [SerializeField] private float _dropDurationMin = 0.5f;
    [SerializeField] private float _dropDurationMax = 1f;

    public float MoveDuration => _moveDuration;

    private Sequence _sequence;

    public void AnimateBubbleMove(Bubble bubble, BubbleTrajectory trajectory, Action onComplete)
    {
        bubble.transform.position = trajectory.FirstPoint.Position;

        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        for (int index = 1; index < trajectory.Points.Count; index++)
        {
            var point = trajectory.Points[index];
            _sequence.Append(bubble.transform.DOMove(point.Position, 0.2f));
        }

        var hexPoint = _hexGridLayout.WorldToHex(trajectory.LastPoint.Position);
        var worldPoint = _hexGridLayout.HexToWorld(hexPoint);

        _sequence.Append(bubble.transform.DOMove(worldPoint, 0.1f));
        _sequence.OnComplete(() => onComplete?.Invoke());
    }

    public void AnimateBubbleSpawn(Bubble bubble)
    {
        bubble.DOKill();
        bubble.transform.localScale = Vector3.zero;
        bubble.transform.DOScale(Vector3.one, _spawnDuration)
            .SetEase(Ease.OutBack);
    }

    public void AnimaterBubbleMove(Bubble bubble, Vector3 worldPoint)
    {
        bubble.DOKill();
        bubble.transform.DOMove(worldPoint, _moveDuration)
            .SetEase(Ease.Linear);
    }

    public void AnimaterBubbleDrop(Bubble bubble, Vector3 worldPoint, Action onComplete)
    {
        var dropDuration = Random.Range(_dropDurationMin, _dropDurationMax);

        bubble.DOKill();
        bubble.transform.DOMove(worldPoint, dropDuration)
            .OnComplete(() => onComplete?.Invoke())
            .SetEase(Ease.InBack);
    }
}
