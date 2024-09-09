using UnityEngine;

namespace BubbleShooter.HexGrids
{
    public class HexGrid
    {
        private int _rowCount;
        private int _columnCount;
        private HexGridCell[,] _gridCells;

        public int RowCount => _rowCount;
        public int ColumnCount => _columnCount;
        public HexGridCell this[int row, int column] => _gridCells[row, column];
        public HexGridCell this[Vector3Int offsetPosition] => _gridCells[offsetPosition.y, offsetPosition.x];

        public HexGrid(int rowCount, int columnCount)
        {
            _rowCount = rowCount;
            _columnCount = columnCount;
            _gridCells = new HexGridCell[rowCount, columnCount];

            for (int row = 0; row < rowCount; row++)
                for (int column = 0; column < columnCount; column++)
                    _gridCells[row, column] = new HexGridCell();
        }
    }
}