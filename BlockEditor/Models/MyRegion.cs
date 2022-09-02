using LevelModel.Models.Components.Art;
using System;

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
                if(!IsComplete())
                    return null;

                return End.Value.X - Start.Value.X;
            }
        }

        public int? Height
        {
            get
            {
                if (!IsComplete())
                    return null;

                return End.Value.Y - Start.Value.Y;
            }
        }

        public bool IsInside(MyPoint? p)
        {
            if(p == null)
                return false;

            if (IsComplete() == false)
                return false;

            if (p.Value.X < Start.Value.X || p.Value.X >= End.Value.X)
                return false;

            if (p.Value.Y < Start.Value.Y || p.Value.Y >= End.Value.Y)
                return false;

            return true;
        }

        public bool IsInside(DrawArt a)
        {
            if (a == null)
                return false;

            if (IsComplete() == false)
                return false;

            var myPoint = new MyPoint(a.X / 30, a.Y / 30);

            if(IsInside(myPoint))
                return true;

            if(a.Movement == null)
                return false;

            var x = a.X;
            var y = a.Y;
            var count = 0;

            foreach(var m in a.Movement)
            {
                if(count % 2 == 0)
                    x += m;
                else
                    y += m;

                count++;
                if (IsInside(new MyPoint(x / 30, y / 30)))
                    return true;
            }

            return false;
        }

        public bool IsComplete()
        {
            if(Point1 == null)
                return false;

            if(Point2 == null)
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
}
