using System;

namespace BubbleShooter.HexGrids
{
    public enum HexCellOffset
    {
        Even = 1,
        Odd = -1
    }

    public static class HexPoints
    {
        public static HexPoint OffsetToHex(OffsetPoint offsetPoint, HexCellLayoutOffset offset)
        {
            switch (offset)
            {
                case HexCellLayoutOffset.EvenRows:
                    return ROffsetToHex(offsetPoint, HexCellOffset.Even);
                case HexCellLayoutOffset.OddRows:
                    return ROffsetToHex(offsetPoint, HexCellOffset.Odd);
                case HexCellLayoutOffset.EvenColumns:
                    return QOffsetToHex(offsetPoint, HexCellOffset.Even);
                case HexCellLayoutOffset.OddColumns:
                    return QOffsetToHex(offsetPoint, HexCellOffset.Odd);
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public static OffsetPoint HexToOffset(HexPoint hexPoint, HexCellLayoutOffset offset)
        {
            switch (offset)
            {
                case HexCellLayoutOffset.EvenRows:
                    return HexToROffset(hexPoint, HexCellOffset.Even);
                case HexCellLayoutOffset.OddRows:
                    return HexToROffset(hexPoint, HexCellOffset.Odd);
                case HexCellLayoutOffset.EvenColumns:
                    return HexToQOffset(hexPoint, HexCellOffset.Even);
                case HexCellLayoutOffset.OddColumns:
                    return HexToQOffset(hexPoint, HexCellOffset.Odd);
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        private static HexPoint ROffsetToHex(OffsetPoint offsetPoint, HexCellOffset offset)
        {
            var column = offsetPoint.column;
            var row = offsetPoint.row;

            var q = column - ((row + (int)offset * (row & 1)) / 2);
            var r = row;
            var s = -q - r;

            return new HexPoint(q, r, s);
        }

        private static HexPoint QOffsetToHex(OffsetPoint offsetPoint, HexCellOffset offset)
        {
            var column = offsetPoint.column;
            var row = offsetPoint.row;

            var q = column;
            var r = row - ((column + (int)offset * (column & 1)) / 2);
            var s = -q - r;
            
            return new HexPoint(q, r, s);
        }

        private static OffsetPoint HexToROffset(HexPoint hexPoint, HexCellOffset offset)
        {
            var q = hexPoint.q;
            var r = hexPoint.r;

            var column = q + ((r + (int)offset * (r & 1)) / 2);
            var row = r;

            return new OffsetPoint(column, row);
        }

        private static OffsetPoint HexToQOffset(HexPoint hexPoint, HexCellOffset offset)
        {
            var q = hexPoint.q;
            var r = hexPoint.r;

            int column = q;
            int row = r + ((q + (int)offset * (q & 1)) / 2);

            return new OffsetPoint(column, row);
        }
    }
}