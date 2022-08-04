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

                return new MyPoint(x, y);
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

    }

    public class UserSelection
    {

        public MyRegion MapRegion { get; }
        public MyRegion ImageRegion { get; }

        public event Action<int?[,]> OnNewSelection;


        public UserSelection()
        {
            MapRegion = new MyRegion();
            ImageRegion = new MyRegion();
        }

        public void Reset()
        {
            MapRegion.Reset();
            ImageRegion.Reset();
        }

        private int?[,] GetSelection(Map map)
        {
            if (map == null || MapRegion == null || !MapRegion.IsComplete())
                return null;

            var start = MapRegion.Start.Value;
            var end   = MapRegion.End.Value;

            var selection = new int?[end.X - start.X, end.Y - start.Y];

            for (int y = start.Y; y < end.Y; y++)
            {
                for (int x = start.X; x < end.X; x++)
                {
                    selection[x - start.X, y - start.Y] = map.Blocks.GetBlockId(x, y);
                }
            }

            return selection;
        }

        public void OnSelectionClick()
        {
            Reset();
        }

        public void OnMouseDown(MyPoint? image, MyPoint? map)
        {
            MapRegion.Point1   = map;
            ImageRegion.Point1 = image;

            MapRegion.Point2   = null;
            ImageRegion.Point2 = null;

        }

        public void OnMouseUp(MyPoint? image, MyPoint? map)
        {
            MapRegion.Point2 = map;
            ImageRegion.Point2 = image;
        }

        public void OnKeydown(Map map)
        {
            var selection = GetSelection(map);
            OnNewSelection?.Invoke(selection);
            Reset();
        }


    }
}
