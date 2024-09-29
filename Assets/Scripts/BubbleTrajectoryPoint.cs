using UnityEngine;

namespace BubbleShooter
{
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
}