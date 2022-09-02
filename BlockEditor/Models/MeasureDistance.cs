using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BlockEditor.Models
{
    public static class MeasureDistance
    {
        public static MyPoint? MapPoint1 { get; set; }
        public static MyPoint? MapPoint2 { get; set; }

        public static MyPoint? ImagePoint1 { get; set; }
        public static MyPoint? ImagePoint2 { get; set; }

        public static void Reset()
        {
            MapPoint1 = null;
            MapPoint2 = null;
            ImagePoint1 = null;
            ImagePoint2 = null;
        }

        public static string GetDistance()
        {
            if(MapPoint1 == null)
                return string.Empty;

            if (MapPoint2 == null)
                return string.Empty;

            var x = MapPoint2.Value.X - MapPoint1.Value.X;
            var y = MapPoint2.Value.Y - MapPoint1.Value.Y;

            if (x == 0 && y == 0) 
                return "You need to click and drag to measure the distance between 2 points.";

            return "The distance is:" + Environment.NewLine 
                + "       " + "X = " + x.ToString(CultureInfo.InvariantCulture) + Environment.NewLine
                + "       " + "Y = " + y.ToString(CultureInfo.InvariantCulture);

        }
    }
}
