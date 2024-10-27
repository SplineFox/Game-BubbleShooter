using BubbleShooter.HexGrids;
using System.Collections.Generic;

namespace BubbleShooter
{
    public class BubbleFloatersDetector
    {
        private HexGrid _hexGrid;

        private HashSet<HexPoint> _visitedPoints;
        private Stack<HexPoint> _searchPoints;
        private List<(HexPoint, Bubble)> _floaters;

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
            _floaters = new();
        }

        public bool TryGetFloaters(out IEnumerable<(HexPoint, Bubble)> floaters)
        {
            _visitedPoints.Clear();
            _searchPoints.Clear();
            _floaters.Clear();

            for (int column = 0; column < _hexGrid.ColumnCount; column++)
            {
                var offsetPoint = new OffsetPoint(column, 0);
                var hexPoint = HexPoints.OffsetToHex(offsetPoint, HexCellLayoutOffset.OddRows);
                _searchPoints.Push(hexPoint);
            }

            while (_searchPoints.TryPop(out var searchPoint))
                ProcessSearchPoint(searchPoint);

            FindFloaters();

            floaters = _floaters;
            return _floaters.Count > 0;
        }

        private void ProcessSearchPoint(HexPoint hexPoint)
        {
            _visitedPoints.Add(hexPoint);

            foreach (var hexDirection in _searchDirections)
            {
                var nextHexPoint = hexPoint.Add(hexDirection);
                if (_visitedPoints.Contains(nextHexPoint))
                    continue;

                if (!_hexGrid.IsPointInBounds(nextHexPoint))
                    continue;

                if (_hexGrid[nextHexPoint].Bubble == null)
                {
                    _visitedPoints.Add(nextHexPoint);
                    continue;
                }

                _searchPoints.Push(nextHexPoint);
            }
        }

        private void FindFloaters()
        {
            for (int row = 0; row < _hexGrid.RowCount; row++)
            {
                for (int column = 0; column < _hexGrid.ColumnCount; column++)
                {
                    var offsetPoint = new OffsetPoint(column, row);
                    var hexPoint = HexPoints.OffsetToHex(offsetPoint, HexCellLayoutOffset.OddRows);
                    
                    if (_visitedPoints.Contains(hexPoint))
                        continue;

                    var bubble = _hexGrid[row, column].Bubble;

                    if (bubble != null)
                        _floaters.Add((hexPoint, bubble));
                }
            }
        }
    }
}