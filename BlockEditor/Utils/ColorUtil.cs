using SkiaSharp;
using System;
using System.Globalization;
using System.Windows.Media;
using static Builders.DataStructures.DTO.ImageDTO;

namespace BlockEditor.Utils
{
    public static class ColorUtil
    {
        public static System.Drawing.Color? GetColorFromBlockOption(string blockOptions)
        {
            if (MyUtil.TryParse(blockOptions, out var result))
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

        public static SKColor GetSKColorFromRGBHex(string input)
        {
            try
            {
                input = input.Trim('#');
                return SKColor.Parse("FF" + input.PadLeft(6, '0'));
            }
            catch
            {
                return SKColors.White;
            }
        }

        public static bool IsColorEqual(SKColor? c1, SKColor? c2, ColorSensitivty sensitivty)
        {
            if (c1 == null || c2 == null)
                return false;

            var distance = GetColorDistance(c1.Value, c2.Value);

            switch (sensitivty)
            {
                case ColorSensitivty.VeryLow: return distance < 300;
                case ColorSensitivty.Low: return distance < 150;
                case ColorSensitivty.Medium: return distance < 75;
                case ColorSensitivty.High: return distance < 40;
                case ColorSensitivty.VeryHigh: return distance < 15;

                default: return distance < 50;
            }
        }

        static double GetColorDistance(SKColor e1, SKColor e2)
        {
            long rmean = ((long)e1.Red + e2.Red) / 2;
            long r = e1.Red - (long)e2.Red;
            long g = e1.Green - (long)e2.Green;
            long b = e1.Blue - (long)e2.Blue;

            return Math.Sqrt((((512 + rmean) * r * r) >> 8) + 4 * g * g + (((767 - rmean) * b * b) >> 8));
        }

        public static SKColor ToSkColor(System.Drawing.Color? c)
        {
            if(c == null)
                return new SKColor();

            return new SKColor(c.Value.R, c.Value.G, c.Value.B);
        }

        public static string ToHexString(SKColor c)
        {
            var c1  = Color.FromRgb(c.Red, c.Green, c.Blue);
            var hex = c1.ToString().Substring(3);

            return hex;
        }

        public static string ToIntString(SKColor c)
        {
            var c1 = Color.FromRgb(c.Red, c.Green, c.Blue);
            var hex = c1.ToString().Substring(3);

            if (int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result))
                return result.ToString(CultureInfo.InvariantCulture);

            return hex;
        }
    }

}
