﻿using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Views.Windows;
using System;
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

        public static bool TryParseDouble(string input, out double result)
        {
            return double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        public static void SetPopUpWindowPosition(Window w)
        {
            var window = App.Current?.MainWindow as MainWindow;

            if(window == null)
                return;

            var padddingX = window.Left + 60;
            var padddingY = window.Top + window.ActualHeight / 7;

            w.Left = padddingX;
            w.Top = padddingY;
            w.MaxHeight = 6 * window.ActualHeight / 7 - 100;
        }

        public static void BlocksOutsideBoundries(int count)
        {
            if(count == 0)
                return;

            var plural = count > 1;
            var blocks = plural ? "blocks" : "block";
            var these  = plural ? "These" : "This";

            MessageUtil.ShowWarning($"This map has {count} {blocks} outside the PR2 boundaries."
                        + Environment.NewLine + Environment.NewLine
                        + $"{these} {blocks} will be ignored.");
        }
    }
}
