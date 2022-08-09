using BlockEditor.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BlockEditor.Views.Controls
{
    public partial class ColorPickerControl : UserControl
    {
        public event Action<string> OnNewColor;

        public ColorPickerControl()
        {
            InitializeComponent();
        }

        public void SetColor(string input)
        {
            if(MyUtils.TryParse(input, out var result))
                input = result.ToString("X6");

            SetColor(ParseInput(input));
        }

        public void SetColor(System.Drawing.Color? c)
        {
            SetColor(Convert(c));
        }

        public void SetColor(Color c)
        {
            ColorButton.Background = new SolidColorBrush(c);
        }

        private System.Drawing.Color? ParseInput(string input)
        {
            System.Drawing.Color? fallback = null;

            if (string.IsNullOrWhiteSpace(input))
                return fallback;

            try
            {
                if(!input.StartsWith("#"))
                    input = "#" + input;

                return System.Drawing.ColorTranslator.FromHtml(input);
            }
            catch
            {
                return fallback;
            }
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            var colorDialog = new System.Windows.Forms.ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Color color = Convert(colorDialog.Color);
                ColorButton.Background = new SolidColorBrush(color);

                var output = System.Drawing.ColorTranslator.ToHtml(Convert(color));
                OnNewColor?.Invoke(output.Substring(1));
            }
        }

        private Color Convert(System.Drawing.Color? c)
        {
            if(c == null)
                return Colors.Transparent;

            return Color.FromArgb(255, c.Value.R, c.Value.G, c.Value.B);
        }

        private System.Drawing.Color Convert(Color c)
        {
            return System.Drawing.Color.FromArgb(255, c.R, c.G, c.B);

        }
    }
}
