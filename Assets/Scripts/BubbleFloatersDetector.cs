using BubbleShooter.HexGrids;
using System.Collections.Generic;
using System.Linq;

namespace BubbleShooter
{
    public class BubbleFloatersDetector
    {
        private HexGrid _hexGrid;

        private HashSet<HexPoint> _visitedPoints;
        private Queue<HexPoint> _searchPoints;
        private List<(HexPoint, Bubble)> _sequence;

        private List<HexPoint> _searchDirections = new()
        {
            HexPoint.One,
            HexPoint.Two,
            HexPoint.Three,
            HexPoint.Four,
            HexPoint.Five,
            HexPoint.Six
        };

        public BubbleFloatersDetector(HexGrid hexGrid)
        {
            _hexGrid = hexGrid;
            _visitedPoints = new();
            _searchPoints = new();
            _sequence = new();
        }

        public bool TryGetFloaters(HexPoint hexPoint, out IEnumerable<(HexPoint, Bubble)> floaters)
        {
            if (!_hexGrid.IsPointInBounds(hexPoint) || _hexGrid[hexPoint].Bubble == null)
            {
                floaters = null;
                return false;
            }

            var bubble = _hexGrid[hexPoint].Bubble;

            _visitedPoints.Clear();
            _searchPoints.Clear();
            _sequence.Clear();

            _visitedPoints.Add(hexPoint);
            _searchPoints.Enqueue(hexPoint);
            _sequence.Add((hexPoint, bubble));

            while (_searchPoints.TryDequeue(out var searchPoint))
                ProcessSearchPoint(searchPoint, bubble.TypeId);

            floaters = _sequence;
            return _sequence.Count() > 2;
        }

        private void ProcessSearchPoint(HexPoint hexPoint, int bubbleTypeId)
        {
            foreach (var hexDirection in _searchDirections)
            {
                var nextHexPoint = hexPoint.Add(hexDirection);
                if (_visitedPoints.Contains(nextHexPoint))
                    continue;

                if (!_hexGrid.IsPointInBounds(nextHexPoint))
                    continue;

                var nextBubble = _hexGrid[nextHexPoint].Bubble;
                if (nextBubble == null || nextBubble.TypeId != bubbleTypeId)
                    continue;

                _visitedPoints.Add(nextHexPoint);
                _searchPoints.Enqueue(nextHexPoint);
                _sequence.Add((nextHexPoint, nextBubble));
            }
        }
    }
}