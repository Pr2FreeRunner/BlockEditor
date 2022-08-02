using BlockEditor.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace BlockEditor.Utils
{

    class MyUtils
    {

        public static MyPoint? GetPosition(IInputElement src, MouseEventArgs e)
        {
            if (src == null || e == null)
                return null;

            var point = e.GetPosition(src);

            var x = (int) point.X;
            var y =  (int) point.Y;

            return new MyPoint(x, y);
        }

        public static bool TryParse(string input, out int result)
        {
            return int.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

    }
}
