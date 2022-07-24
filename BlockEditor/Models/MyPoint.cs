using System.Windows;

namespace BlockEditor.Models
{
    public struct MyPoint 
    {
        public int X { get; set; }
        public int Y { get; set; }

        public MyPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator MyPoint(Point p)
        {
            var x = (int)p.X;
            var y = (int)p.Y;

            return new MyPoint(x, y);
        }
    }
}
