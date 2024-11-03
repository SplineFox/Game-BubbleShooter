using System.Linq;
using DG.Tweening;
using UnityEngine;
using BubbleShooter;
using BubbleShooter.HexGrids;
using Random = UnityEngine.Random;

public class BubbleAnimator : MonoBehaviour
{
    [SerializeField] private float _spawnDuration = 0.4f;
    [SerializeField] private float _moveDuration = 0.2f;
    [SerializeField] private float _dropDurationMin = 0.5f;
    [SerializeField] private float _dropDurationMax = 1f;

    public float MoveDuration => _moveDuration;

    public Sequence CreateBubbleFlightSequence(Bubble bubble, BubbleTrajectory trajectory, Vector3 worldPoint)
    {
        bubble.transform.position = trajectory.FirstPoint.Position;

        var sequence = DOTween.Sequence();
        
        for (int index = 1; index < trajectory.Points.Count; index++)
        {
            var point = trajectory.Points[index];
            sequence.Append(bubble.transform.DOMove(point.Position, 0.2f));
        }
        sequence.Append(bubble.transform.DOMove(worldPoint, 0.1f));

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

        var dropDuration = Random.Range(_dropDurationMin, _dropDurationMax);
        var tween = bubble.transform
            .DOMove(worldPoint, dropDuration)
            .SetEase(Ease.InBack);

        return tween;
    }
}
