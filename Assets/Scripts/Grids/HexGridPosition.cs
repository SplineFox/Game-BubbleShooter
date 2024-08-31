using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace BubbleShoter.Grids
{
    public struct HexGridPosition : IEquatable<HexGridPosition>, IFormattable
    {
        private static readonly HexGridPosition zeroPosition = new HexGridPosition(0, 0, 0);

        private static readonly HexGridPosition qNextPosition = new HexGridPosition(0, 1, -1);
        private static readonly HexGridPosition rNextPosition = new HexGridPosition(1, 0, -1);
        private static readonly HexGridPosition sNextPosition = new HexGridPosition(1, -1, 0);

        private static readonly HexGridPosition qPrevPosition = new HexGridPosition(0, -1, 1);
        private static readonly HexGridPosition rPrevPosition = new HexGridPosition(-1, 0, 1);
        private static readonly HexGridPosition sPrevPosition = new HexGridPosition(-1, 1, 0);

        public int q { get; private set; }
        public int r { get; private set; }
        public int s { get; private set; }

        /// <summary>
        /// Shorthand for writing HexGridPosition(0, 0, 0).
        /// </summary>
        public static HexGridPosition zero => zeroPosition;

        /// <summary>
        /// Shorthand for writing HexGridPosition(0, 1, -1).
        /// </summary>
        public static HexGridPosition qNext => qNextPosition;

        /// <summary>
        /// Shorthand for writing HexGridPosition(1, 0, -1).
        /// </summary>
        public static HexGridPosition rNext => rNextPosition;

        /// <summary>
        /// Shorthand for writing HexGridPosition(1, -1, 0).
        /// </summary>
        public static HexGridPosition sNext => sNextPosition;

        /// <summary>
        /// Shorthand for writing HexGridPosition(0, -1, 1).
        /// </summary>
        public static HexGridPosition qPrev => qPrevPosition;

        /// <summary>
        /// Shorthand for writing HexGridPosition(-1, 0, 1).
        /// </summary>
        public static HexGridPosition rPrev => rPrevPosition;

        /// <summary>
        /// Shorthand for writing HexGridPosition(-1, 1, 0).
        /// </summary>
        public static HexGridPosition sPrev => sPrevPosition;

        public HexGridPosition(int q, int r, int s)
        {
            this.q = q;
            this.r = r;
            this.s = s;

            if (q + r + s != 0)
                throw new ArgumentException("q + r + s must be 0");
        }

        public HexGridPosition(int q, int r)
        {
            this.q = q;
            this.r = r;
            this.s = -q - r;

            if (q + r + s != 0)
                throw new ArgumentException("q + r + s must be 0");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HexGridPosition operator *(HexGridPosition a, int b)
        {
            return new HexGridPosition(a.q * b, a.r * b, a.s * b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HexGridPosition operator /(HexGridPosition a, int b)
        {
            return new HexGridPosition(a.q / b, a.r / b, a.s / b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HexGridPosition operator +(HexGridPosition a, HexGridPosition b)
        {
            return new HexGridPosition(a.q + b.q, a.r + b.r, a.s + b.s);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HexGridPosition operator -(HexGridPosition a, HexGridPosition b)
        {
            return new HexGridPosition(a.q - b.q, a.r - b.r, a.s - b.s);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(HexGridPosition a, HexGridPosition b)
        {
            return a.q == b.q && a.r == b.r && a.s == b.s;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(HexGridPosition a, HexGridPosition b)
        {
            return !(a == b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is HexGridPosition other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(HexGridPosition obj)
        {
            return this == obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return q.GetHashCode() ^ (r.GetHashCode() << 2) ^ (s.GetHashCode() >> 2);
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

            return string.Format("({0}, {1}, {2})", q.ToString(format, formatProvider), r.ToString(format, formatProvider), s.ToString(format, formatProvider));
        }
    }
}