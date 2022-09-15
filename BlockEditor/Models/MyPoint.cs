using System.Diagnostics;
using System.Globalization;

namespace BlockEditor.Models
{
    [DebuggerDisplay("{X}, {Y}")]
    public struct MyPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public MyPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object o)
        {
            if(o is MyPoint p)
                return Equals(p);

            return false;
        }

        public bool Equals(MyPoint p)
        {
            return this == p;
        }

        public override int GetHashCode() => (X, Y).GetHashCode();

        public override string ToString() => "{X=" + X.ToString(CultureInfo.CurrentCulture) + ",Y=" + Y.ToString(CultureInfo.CurrentCulture) + "}";

        public static bool operator ==(MyPoint p1, MyPoint p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(MyPoint p1, MyPoint p2)
        {
            return !(p1 == p2);
        }

        public MyPoint Move(int x, int y)
        {
            return new MyPoint(X + x, Y + y);
        }
    }
}
