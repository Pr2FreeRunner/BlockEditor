using System;
using System.Windows.Media;

namespace BlockEditor.Utils
{
    public static class ColorUtil
    {
        public static System.Drawing.Color? GetColorFromBlockOption(string blockOptions)
        {
            if (MyUtils.TryParse(blockOptions, out var result))
                blockOptions = result.ToString("X6");

            return GetColorFromHex(blockOptions);
        }

        public static System.Drawing.Color? GetColorFromHex(string input)
        {
            System.Drawing.Color? fallback = null;

            if (string.IsNullOrWhiteSpace(input) || string.Equals(input, "#", StringComparison.InvariantCultureIgnoreCase))
                return fallback;

            try
            {
                if (!input.StartsWith("#"))
                    input = "#" + input;

                return System.Drawing.ColorTranslator.FromHtml(input);
            }
            catch
            {
                return fallback;
            }
        }

        public static Color? Convert(System.Drawing.Color? c)
        {
            if (c == null)
                return null;

            return Color.FromArgb(255, c.Value.R, c.Value.G, c.Value.B);
        }

        public static System.Drawing.Color Convert(Color c)
        {
            return System.Drawing.Color.FromArgb(255, c.R, c.G, c.B);

        }
    }

}
