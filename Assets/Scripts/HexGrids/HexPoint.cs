using System;
using System.Globalization;
using UnityEngine;

namespace BubbleShooter.HexGrids
{
    public struct HexPoint
    {
        public readonly int q;

        public readonly int r;

        public readonly int s;

        private static readonly HexPoint _zero = new HexPoint(0, 0, 0);

        private static readonly HexPoint _one = new HexPoint(+1, -1, 0);

        private static readonly HexPoint _two = new HexPoint(+1, 0, -1);

        private static readonly HexPoint _three = new HexPoint(0, +1, -1);

        private static readonly HexPoint _four = new HexPoint(-1, +1, 0);

        private static readonly HexPoint _five = new HexPoint(-1, 0, +1);

        private static readonly HexPoint _six = new HexPoint(0, -1, +1);

        public static HexPoint Zero => _zero;

        public static HexPoint One => _one;

        public static HexPoint Two => _two;

        public static HexPoint Three => _three;

        public static HexPoint Four => _four;

        public static HexPoint Five => _five;

        public static HexPoint Six => _six;

        public HexPoint(int q, int r, int s)
        {
            this.q = q;
            this.r = r;
            this.s = s;

            if (q + r + s != 0)
                throw new ArgumentException("q + r + s must be 0");
        }

        public HexPoint Add(HexPoint second)
        {
            return new HexPoint(q + second.q, r + second.r, s + second.s);
        }

        public HexPoint Subtract(HexPoint second)
        {
            return new HexPoint(q - second.q, r - second.r, s - second.s);
        }

        public HexPoint Scale(int value)
        {
            return new HexPoint(q * value, r * value, s * value);
        }

        public HexPoint RotateLeft()
        {
            return new HexPoint(-s, -q, -r);
        }

        public HexPoint RotateRight()
        {
            return new HexPoint(-r, -s, -q);
        }

        public int Length()
        {
            return (int)((Mathf.Abs(q) + Mathf.Abs(r) + Mathf.Abs(s)) / 2);
        }

        public int Distance(HexPoint second)
        {
            return Subtract(second).Length();
        }

        public override string ToString()
        {
            return ToString(null, null);
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                formatProvider = CultureInfo.InvariantCulture.NumberFormat;
            }

            return string.Format("({0}, {1}, {2})",
                q.ToString(format, formatProvider),
                r.ToString(format, formatProvider),
                s.ToString(format, formatProvider));
        }
    }
}