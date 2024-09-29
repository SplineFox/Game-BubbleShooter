using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BubblePhysics
{
    private List<RaycastHit2D> _raycastHits;
    private ContactFilter2D _contactFilter;
    private float _bubbleRadius;

    private int _bubbleLayer;
    private int _wallLayer;

    public BubblePhysics(float bubbleRadius, int bubbleLayer, int wallLayer)
    {
        _raycastHits = new();
        _contactFilter = new();
        
        _bubbleRadius = bubbleRadius;
        _bubbleLayer = bubbleLayer;
        _wallLayer = wallLayer;
    }

    public BubbleTrajectory FindTrajectory(Vector2 origin, Vector2 direction)
    {
        var point = new BubbleTrajectoryPoint(origin, direction);
        var trajectory = new BubbleTrajectory(point);
        var destinationFound = false;

        var iteration = 0;
        do
        {
            iteration++;
            point = FindNextPoint(point, out var found);
            trajectory.AddPoint(point);
            destinationFound |= found;
        }
        while (!destinationFound && iteration < 10);

        return trajectory;
    }

    private BubbleTrajectoryPoint FindNextPoint(BubbleTrajectoryPoint point, out bool destinationFound)
    {
        _raycastHits.Clear();
        Physics2D.CircleCast(point.Position, _bubbleRadius, point.Direction, _contactFilter, _raycastHits);

        if (_raycastHits.Count == 0)
        {
            destinationFound = true;
            return default;
        }

        destinationFound = false;
        foreach (var hit in _raycastHits)
        {
            destinationFound |= hit.collider.gameObject.layer == _bubbleLayer;
        }

        var firstHit = _raycastHits.First();
        var position = firstHit.point + firstHit.normal * _bubbleRadius;
        var direction = destinationFound? Vector2.zero : Vector2.Reflect(point.Direction, firstHit.normal);

        return new BubbleTrajectoryPoint(position, direction);
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
