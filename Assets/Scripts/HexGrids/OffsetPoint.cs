namespace BubbleShooter.HexGrids
{
    public struct OffsetPoint
    {
        public readonly int column;
        public readonly int row;

        public OffsetPoint(int column, int row)
        {
            this.column = column;
            this.row = row;
        }
    }
}