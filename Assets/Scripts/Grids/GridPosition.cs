using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace BubbleShoter.Grids
{
    public struct GridPosition : IEquatable<GridPosition>, IFormattable
    {
        private static readonly GridPosition zeroPosition = new GridPosition(0, 0);
        private static readonly GridPosition onePosition = new GridPosition(1, 1);
        private static readonly GridPosition upPosition = new GridPosition(0, 1);
        private static readonly GridPosition downPosition = new GridPosition(0, -1);
        private static readonly GridPosition leftPosition = new GridPosition(-1, 0);
        private static readonly GridPosition rightPosition = new GridPosition(1, 0);

        public int row;
        public int column;

        /// <summary>
        /// Shorthand for writing GridPosition(0, 0).
        /// </summary>
        public static GridPosition zero => zeroPosition;

        /// <summary>
        /// Shorthand for writing GridPosition(1, 1).
        /// </summary>
        public static GridPosition one => onePosition;

        /// <summary>
        /// Shorthand for writing GridPosition(-1, 0).
        /// </summary>
        public static GridPosition up => upPosition;

        /// <summary>
        /// Shorthand for writing GridPosition(1, 0).
        /// </summary>
        public static GridPosition down => downPosition;

        /// <summary>
        /// Shorthand for writing GridPosition(0, -1).
        /// </summary>
        public static GridPosition left => leftPosition;

        /// <summary>
        /// Shorthand for writing GridPosition(0, 1).
        /// </summary>
        public static GridPosition right => rightPosition;

        /// <summary>
        /// Shorthand for getting next up GridPosition.
        /// </summary>
        public GridPosition nextUp => this + up;

        /// <summary>
        /// Shorthand for getting next down GridPosition.
        /// </summary>
        public GridPosition nextDown => this + down;

        /// <summary>
        /// Shorthand for getting next left GridPosition.
        /// </summary>
        public GridPosition nextLeft => this + left;

        /// <summary>
        /// Shorthand for getting next right GridPosition.
        /// </summary>
        public GridPosition nextRight => this + right;


        public GridPosition(int rowIndex, int columnIndex)
        {
            row = rowIndex;
            column = columnIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GridPosition operator +(GridPosition a, GridPosition b)
        {
            return new GridPosition(a.row + b.row, a.column + b.column);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GridPosition operator -(GridPosition a, GridPosition b)
        {
            return new GridPosition(a.row - b.row, a.column - b.column);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(GridPosition a, GridPosition b)
        {
            return a.row == b.row && a.column == b.column;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(GridPosition a, GridPosition b)
        {
            return a.row != b.row || a.column != b.column;
        }

        /// <summary>
        /// Returns a position that is made from the smallest components of two position.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GridPosition Min(GridPosition lhs, GridPosition rhs)
        {
            return new GridPosition(Mathf.Min(lhs.row, rhs.row), Mathf.Min(lhs.column, rhs.column));
        }

        /// <summary>
        /// Returns a position that is made from the largest components of two position.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GridPosition Max(GridPosition lhs, GridPosition rhs)
        {
            return new GridPosition(Mathf.Max(lhs.row, rhs.row), Mathf.Max(lhs.column, rhs.column));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is GridPosition other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EqualsRow(GridPosition other)
        {
            return row == other.row;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EqualsCollumn(GridPosition other)
        {
            return column == other.column;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(GridPosition other)
        {
            return EqualsRow(other) && EqualsCollumn(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return row.GetHashCode() ^ (column.GetHashCode() << 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return ToString(null, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "D";
            }

            if (formatProvider == null)
            {
                formatProvider = CultureInfo.InvariantCulture.NumberFormat;
            }

            return string.Format("({0}, {1})", column.ToString(format, formatProvider), column.ToString(format, formatProvider));
        }
    }
}