using System.Linq;
using DG.Tweening;
using UnityEngine;
using BubbleShooter;
using BubbleShooter.HexGrids;

public class BubbleAnimator : MonoBehaviour
{
    [SerializeField] private float _moveDistancePerSecond = 35f;
    [SerializeField] private float _dropDistancePerSecond = 15f;

    [SerializeField] private float _spawnDuration = 0.4f;
    [SerializeField] private float _moveDuration = 0.2f;

    public float MoveDuration => _moveDuration;

    public Sequence CreateBubbleFlightSequence(Bubble bubble, BubbleTrajectory trajectory, Vector3 worldPoint)
    {
        var sequence = DOTween.Sequence();

        var distance = 0f;
        var duration = 0f;
        var previousPosition = trajectory.FirstPoint.Position;

        bubble.transform.position = trajectory.FirstPoint.Position;

        for (int index = 1; index < trajectory.Points.Count - 1; index++)
        {
            var point = trajectory.Points[index];
            
            distance = Vector2.Distance(previousPosition, point.Position);
            duration = distance / _moveDistancePerSecond;

            sequence.Append(bubble.transform.DOMove(point.Position, duration).SetEase(Ease.Linear));
            previousPosition = point.Position;
        }

        var direction = (trajectory.LastPoint.Position - previousPosition).normalized;
        var lastPoint = trajectory.LastPoint.Position - direction * 0.2f;

        distance = Vector2.Distance(previousPosition, lastPoint);
        duration = distance / _moveDistancePerSecond;

        sequence.Append(bubble.transform.DOMove(lastPoint, duration).SetEase(Ease.Linear));
        sequence.Append(bubble.transform.DOMove(worldPoint, 0.15f).SetEase(Ease.Linear));
        sequence.Insert(duration, bubble.transform.DOScale(0.75f, 0.075f).SetLoops(2, LoopType.Yoyo));

        return sequence;
    }

    public Tween CreateBubbleSpawnTween(Bubble bubble)
    {
        bubble.DOKill();
        bubble.transform.localScale = Vector3.zero;
        
        var tween = bubble.transform
            .DOScale(Vector3.one, _spawnDuration)
            .SetEase(Ease.OutBack);

        return tween;
    }

    public Tween CreateBubbleMoveTween(Bubble bubble, Vector3 worldPoint)
    {
        bubble.DOKill();

        var tween = bubble.transform
            .DOMove(worldPoint, _moveDuration)
            .SetEase(Ease.Linear);

        return tween;
    }

    public Tween CreateBubbleDropTween(Bubble bubble, Vector3 worldPoint)
    {
        bubble.DOKill();

        var distance = Vector2.Distance(bubble.transform.position, worldPoint);
        var duration = distance / _dropDistancePerSecond;
        var tween = bubble.transform
            .DOMove(worldPoint, duration)
            .SetEase(Ease.InBack);

        return tween;
    }
}
