using UnityEngine;

namespace BubbleShooter.HexGrids
{
    public class HexGridLayout : MonoBehaviour
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private HexCellLayoutOffset _offset;

        public Vector2 CellSize => _grid.cellSize;

        public Vector3 HexToWorld(HexPoint hexPoint)
        {
            var offsetPoint = HexPoints.HexToOffset(hexPoint, _offset);
            var cellPoint = new Vector3Int(offsetPoint.column, offsetPoint.row);
            return _grid.CellToWorld(cellPoint);
        }

        public HexPoint WorldToHex(Vector3 worldPoint)
        {
            var cellPoint = _grid.WorldToCell(worldPoint);
            var offsetPoint = new OffsetPoint(cellPoint.x, cellPoint.y);
            return HexPoints.OffsetToHex(offsetPoint, _offset);
        }

        public Vector3 OffsetToWorld(OffsetPoint offsetPoint)
        {
            var cellPoint = new Vector3Int(offsetPoint.column, offsetPoint.row);
            return _grid.CellToWorld(cellPoint);
        }

        public OffsetPoint WorldToOffset(Vector3 worldPoint)
        {
            var cellPoint = _grid.WorldToCell(worldPoint);
            var offsetPoint = new OffsetPoint(cellPoint.x, cellPoint.y);
            return offsetPoint;
        }
    }

    public class HexGridCell
    {
        public Bubble Bubble;
    }
}