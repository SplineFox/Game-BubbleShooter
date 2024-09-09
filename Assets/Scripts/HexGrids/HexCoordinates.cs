using System;
using UnityEngine;

namespace BubbleShooter.HexGrids
{
    public enum HexCellOffset
    {
        Even = 1,
        Odd = -1
    }

    public static class HexCoordinates
    {
        public static Vector3Int RowsToCube(HexCellOffset offset, Vector3Int offsetCoordinate)
        {
            var column = offsetCoordinate.x;
            var row = offsetCoordinate.y;

            var q = column - ((row + (int)offset * (row & 1)) / 2);
            var r = row;
            var s = -q - r;

            return new Vector3Int(q, r, s);
        }

        public static Vector3Int ColumnsToCube(HexCellOffset offset, Vector3Int offsetCoordinate)
        {
            var column = offsetCoordinate.x;
            var row = offsetCoordinate.y;

            var q = column;
            var r = row - ((column + (int)offset * (column & 1)) / 2);
            var s = -q - r;
            
            return new Vector3Int(q, r, s);
        }

        public static Vector3Int OffsetToCube(Vector3Int offsetCoordinate, HexCellLayoutOffset offset)
        {
            switch (offset)
            {
                case HexCellLayoutOffset.EvenRows:
                    return RowsToCube(HexCellOffset.Even, offsetCoordinate);
                case HexCellLayoutOffset.OddRows:
                    return RowsToCube(HexCellOffset.Odd, offsetCoordinate);
                case HexCellLayoutOffset.EvenColumns:
                    return ColumnsToCube(HexCellOffset.Even, offsetCoordinate);
                case HexCellLayoutOffset.OddColumns:
                    return ColumnsToCube(HexCellOffset.Odd, offsetCoordinate);
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public static Vector3Int CubeToRows(HexCellOffset offset, Vector3Int cubeCoordinate)
        {
            var q = cubeCoordinate.x;
            var r = cubeCoordinate.y;

            var column = q + ((r + (int)offset * (r & 1)) / 2);
            var row = r;

            return new Vector3Int(column, row);
        }

        public static Vector3Int CubeToColumns(HexCellOffset offset, Vector3Int cubeCoordinate)
        {
            var q = cubeCoordinate.x;
            var r = cubeCoordinate.y;

            int column = q;
            int row = r + ((q + (int)offset * (q & 1)) / 2);

            return new Vector3Int(column, row);
        }

        public static Vector3Int CubeToOffset(Vector3Int cubeCoordinate, HexCellLayoutOffset offset)
        {
            switch (offset)
            {
                case HexCellLayoutOffset.EvenRows:
                    return CubeToRows(HexCellOffset.Even, cubeCoordinate);
                case HexCellLayoutOffset.OddRows:
                    return CubeToRows(HexCellOffset.Odd, cubeCoordinate);
                case HexCellLayoutOffset.EvenColumns:
                    return CubeToColumns(HexCellOffset.Even, cubeCoordinate);
                case HexCellLayoutOffset.OddColumns:
                    return CubeToColumns(HexCellOffset.Odd, cubeCoordinate);
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
}