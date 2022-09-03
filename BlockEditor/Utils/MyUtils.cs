using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Views.Windows;
using System;
using System.Globalization;
using System.Linq;
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

        public static bool TryParseDouble(string input, out double result)
        {
            return double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        public static void BlocksOutsideBoundries(int count)
        {
            if(count == 0)
                return;

            var plural = count > 1;
            var blocks = plural ? "blocks" : "block";
            var these  = plural ? "These" : "This";

            MessageUtil.ShowWarning($"This map has {count} {blocks} outside the Editor's boundaries."
                        + Environment.NewLine + Environment.NewLine
                        + $"{these} {blocks} will be ignored.");
        }

        public static string InsertSpaceBeforeCapitalLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            if (string.Equals("ID", input, StringComparison.InvariantCultureIgnoreCase))
                return input;

            return string.Concat(input.ToString().Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        }
    }
}
