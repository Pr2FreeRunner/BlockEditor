using System;
using System.Diagnostics;
using System.Globalization;

namespace BlockEditor.Models
{
    public class MyRegion
    {
        public MyPoint? Point1 { get; set; }

        public MyPoint? Point2 { get; set; }

        public  MyPoint? Start 
        {
            get
            {
                if (Point1 == null && Point2 == null)
                    return null;

                if (Point2 == null)
                    return Point1;

                if (Point1 == null)
                    return Point2;

                var x = Math.Min(Point1.Value.X, Point2.Value.X);
                var y = Math.Min(Point1.Value.Y, Point2.Value.Y);

                return new MyPoint(x, y);
            }
        }

        public MyPoint? End
        {
            get
            {
                if (Point1 == null || Point2 == null)
                    return null;

                var x = Math.Max(Point1.Value.X, Point2.Value.X);
                var y = Math.Max(Point1.Value.Y, Point2.Value.Y);

                return new MyPoint(x + 1, y + 1);
            }
        }

        public int? Width
        {
            get
            {
                if(!this.IsComplete())
                    return null;

                return End.Value.X - Start.Value.X;
            }
        }

        public int? Height
        {
            get
            {
                if (!this.IsComplete())
                    return null;

                return End.Value.Y - Start.Value.Y;
            }
        }

        public bool IsInside(MyPoint? p)
        {
            if(p == null)
                return false;

            if (this.IsComplete() == false)
                return false;

            if (p.Value.X < Start.Value.X || p.Value.X >= End.Value.X)
                return false;

            if (p.Value.Y < Start.Value.Y || p.Value.Y >= End.Value.Y)
                return false;

            return true;
        }

        public void Reset()
        {
            Point1 = null;
            Point2 = null;
        }

        public MyRegion Copy()
        {
            var r = new MyRegion();

            r.Point1 = Point1;
            r.Point2 = Point2;

            return r;
        }

    }


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
            if (o is MyPoint p)
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
