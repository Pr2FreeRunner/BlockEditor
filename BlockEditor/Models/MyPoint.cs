using System.Diagnostics;

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

    }
}
