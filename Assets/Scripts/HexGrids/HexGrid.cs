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
        public HexGridCell this[OffsetPoint offsetPoint] => this[offsetPoint.row, offsetPoint.column];
        public HexGridCell this[HexPoint hexPoint] => this[HexPoints.HexToOffset(hexPoint, HexCellLayoutOffset.OddRows)];

        public HexGrid(int rowCount, int columnCount)
        {
            _rowCount = rowCount;
            _columnCount = columnCount;
            _gridCells = new HexGridCell[rowCount, columnCount];

            for (int row = 0; row < rowCount; row++)
                for (int column = 0; column < columnCount; column++)
                    _gridCells[row, column] = new HexGridCell();
        }

        public bool IsPointInBounds(int row, int column)
        {
            return row >= 0
                && column >= 0
                && row < _rowCount
                && column < _columnCount;
        }

        public bool IsPointInBounds(OffsetPoint offsetPoint)
        {
            return IsPointInBounds(offsetPoint.row, offsetPoint.column);
        }

        public bool IsPointInBounds(HexPoint hexPoint)
        {
            return IsPointInBounds(HexPoints.HexToOffset(hexPoint, HexCellLayoutOffset.OddRows));
        }
    }
}