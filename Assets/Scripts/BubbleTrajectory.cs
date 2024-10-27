using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BubbleShooter
{
    public class BubbleTrajectory : IEnumerable<BubbleTrajectoryPoint>
    {
        private List<BubbleTrajectoryPoint> _points;
        public BubbleTrajectoryPoint FirstPoint => _points.First();
        public BubbleTrajectoryPoint LastPoint => _points.Last();
        public IReadOnlyList<BubbleTrajectoryPoint> Points => _points;

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
}