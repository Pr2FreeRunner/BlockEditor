using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace BlockEditor.Utils
{

    class MyUtils
    {

        public static Point? GetPosition(IInputElement src, MouseEventArgs e)
        {
            if (src == null || e == null)
                return null;

            var point = e.GetPosition(src);

            var x = point.X;
            var y = point.Y;

            return new Point(x, y);
        }

        public static bool TryParse(string input, out int result)
        {
            return int.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

    }
}
