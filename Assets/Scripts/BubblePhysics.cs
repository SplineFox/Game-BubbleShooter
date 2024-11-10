using UnityEngine;

namespace BubbleShooter
{
    public class BubblePhysics : MonoBehaviour
    {
        private const int MAX_ITERATIONS_COUNT = 100;

        private float _bubbleCastRadius;
        private float _bubbleRadius;
        private int _bubbleLayer;

        private RaycastHit2D _raycastHit;

        public void Setup(float bubbleCastRadius, float bubbleRadius, int bubbleLayer)
        {
            _bubbleCastRadius = bubbleCastRadius;
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

            _raycastHit = Physics2D.CircleCast(point.Position, _bubbleCastRadius, point.Direction);
            if (_raycastHit.collider == null)
            {
                nextPoint = default;
                return false;
            }

            var bubbleReached = _raycastHit.collider.gameObject.layer == _bubbleLayer;
            var position = _raycastHit.point + _raycastHit.normal * _bubbleCastRadius;
            var direction = bubbleReached ? Vector2.zero : Vector2.Reflect(point.Direction, _raycastHit.normal);

            nextPoint = new BubbleTrajectoryPoint(position, direction);
            return true;
        }

        private void OnDrawGizmos()
        {
            //if (_raycastHits == null)
            //    return;
            //
            //var point = Vector2.zero;
            //var normal = Vector2.zero;
            //foreach (var hit in _raycastHits)
            //{
            //    point += hit.point;
            //    normal += hit.normal;
            //
            //    Gizmos.color = Color.magenta;
            //    Gizmos.DrawWireSphere(hit.point, 0.02f);
            //    GizmosUtils.DrawArrow(hit.point, hit.point + hit.normal);
            //}
            //
            //point /= _raycastHits.Count;
            //normal.Normalize();
            //
            //Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(point, 0.02f);
            //GizmosUtils.DrawArrow(point, point + normal);
        }
    }
}