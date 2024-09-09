using UnityEngine;

namespace BubbleShooter.HexGrids
{
    public class HexGridLayout : MonoBehaviour
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private HexCellLayoutOffset _offset;

        public Vector3 CubeToWorld(Vector3Int cubePoint)
        {
            var offsetCoordinate = HexCoordinates.CubeToOffset(cubePoint, _offset);
            return _grid.CellToWorld(offsetCoordinate);
        }

        public Vector3Int WorldToCube(Vector3 worldPoint)
        {
            var offsetCoordinate = _grid.WorldToCell(worldPoint);
            return HexCoordinates.OffsetToCube(offsetCoordinate, _offset);
        }

        public Vector3 OffsetToWorld(Vector3Int offsetPoint)
        {
            return _grid.CellToWorld(offsetPoint);
        }

        public Vector3Int WorldToOffset(Vector3 worldPoint)
        {
            return _grid.WorldToCell(worldPoint);
        }
    }

    public class HexGridCell
    {
        public Bubble Bubble;
    }
}