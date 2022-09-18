using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Views.Windows;
using System;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Input;

namespace BlockEditor.Utils
{

    class MyUtil
    {

        public static bool HasInternet()
        {
            try
            {
                return NetworkInterface.GetIsNetworkAvailable();
            }
            catch
            {
                return false;
            }
        }

        public static MyPoint DipToPixels(Point pt)
        {
            var dpiScale = App.MyMainWindow.DpiScale;
            return new MyPoint((int)(pt.X * dpiScale.X), (int)(pt.Y * dpiScale.Y));
        }

        public static MyPoint? GetPosition(IInputElement src, MouseEventArgs e)
        {
            if (src == null || e == null)
                return null;

            var point = e.GetPosition(src);

            return DipToPixels(point);
        }

        public static bool TryParse(string input, out int result)
        {
            return int.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        public static bool TryParseDouble(string input, out double result)
        {
            return double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        public static void SetPopUpWindowPosition(Window w)
        {
            var mainWindow = App.Current?.MainWindow as MainWindow;

            if (mainWindow == null)
                return;

            var x = 60;
            var y = 150;

            w.Left = mainWindow.WindowState == WindowState.Maximized ? x : mainWindow.Left + x; 
            w.Top  = mainWindow.WindowState == WindowState.Maximized ? y : mainWindow.Top  + y;
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
