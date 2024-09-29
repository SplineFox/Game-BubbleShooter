using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BubblePhysics
{
    private const int MAX_ITERATIONS_COUNT = 100;

    private List<RaycastHit2D> _raycastHits;
    private ContactFilter2D _contactFilter;
    private float _bubbleRadius;

    private int _bubbleLayer;

    public BubblePhysics(float bubbleRadius, int bubbleLayer)
    {
        _raycastHits = new();
        _contactFilter = new();
        
        _bubbleRadius = bubbleRadius;
        _bubbleLayer = bubbleLayer;
    }

    public BubbleTrajectory FindTrajectory(Vector2 origin, Vector2 direction)
    {
        var point = new BubbleTrajectoryPoint(origin, direction);
        var trajectory = new BubbleTrajectory(point);

        var iteration = 0;
        while (TryFindNextPoint(point, out var nextPoint) && iteration < MAX_ITERATIONS_COUNT)
        {
            iteration++;
            point = nextPoint;
            trajectory.AddPoint(nextPoint);
        }

        return trajectory;
    }

    private bool TryFindNextPoint(BubbleTrajectoryPoint point, out BubbleTrajectoryPoint nextPoint)
    {
        if (point.Direction.magnitude == 0)
        {
            nextPoint = default;
            return false;
        }

        _raycastHits.Clear();
        Physics2D.CircleCast(point.Position, _bubbleRadius, point.Direction, _contactFilter, _raycastHits);

        if (_raycastHits.Count == 0)
        {
            nextPoint = default;
            return false;
        }

        var bubbleReached = false;
        foreach (var hit in _raycastHits)
        {
            bubbleReached |= hit.collider.gameObject.layer == _bubbleLayer;
        }

        var firstHit = _raycastHits.First();
        var position = firstHit.point + firstHit.normal * _bubbleRadius;
        var direction = bubbleReached ? Vector2.zero : Vector2.Reflect(point.Direction, firstHit.normal);

        nextPoint = new BubbleTrajectoryPoint(position, direction);
        return true;
    }
}

public class BubbleTrajectory : IEnumerable<BubbleTrajectoryPoint>
{
    private List<BubbleTrajectoryPoint> _points;
    public BubbleTrajectoryPoint FirstPoint => _points.First();
    public BubbleTrajectoryPoint LastPoint => _points.Last();

    public BubbleTrajectory(BubbleTrajectoryPoint originPoint)
    {
        _points = new();
        AddPoint(originPoint);
    }

    public void AddPoint(BubbleTrajectoryPoint point)
    { 
        _points.Add(point); 
    }

    public IEnumerator<BubbleTrajectoryPoint> GetEnumerator()
    {
        return _points.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _points.GetEnumerator();
    }
}

public struct BubbleTrajectoryPoint
{
    public Vector2 Position;
    public Vector2 Direction;

    public BubbleTrajectoryPoint(Vector2 position, Vector2 direction)
    {
        Position = position;
        Direction = direction;
    }
}
