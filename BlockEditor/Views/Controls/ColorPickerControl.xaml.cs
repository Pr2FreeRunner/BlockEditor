using BlockEditor.Utils;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BlockEditor.Views.Controls
{
    public partial class ColorPickerControl : UserControl
    {
        public event Action<string> OnNewColor;
        private bool _initComplete;
        private bool _ingoreNextUpdate;

        public ColorPickerControl()
        {
            InitializeComponent();
            _initComplete = true;
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

        public void SetColor(Color? c)
        {
            if(c == null)
            {
                ColorButton.Background = new SolidColorBrush(Colors.Transparent);

                if (_ingoreNextUpdate)
                    _ingoreNextUpdate = false;
                else
                    tbColor.Text = string.Empty;

                OnNewColor?.Invoke(string.Empty);
            }
            else
            {
                ColorButton.Background = new SolidColorBrush(c.Value);

                if(_ingoreNextUpdate)
                    _ingoreNextUpdate = false;
                else
                    tbColor.Text = "#" + c.Value.ToString().Substring(3);

                OnNewColor?.Invoke(System.Drawing.ColorTranslator.ToHtml(Convert(c.Value)).Substring(1));
            }
        }

        private System.Drawing.Color? ParseInput(string input)
        {
            System.Drawing.Color? fallback = null;

            if (string.IsNullOrWhiteSpace(input) || string.Equals(input, "#", StringComparison.InvariantCultureIgnoreCase))
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
                Color? color = Convert(colorDialog.Color);
                SetColor(color);
            }
        }

        private Color? Convert(System.Drawing.Color? c)
        {
            if(c == null)
                return null;

            return Color.FromArgb(255, c.Value.R, c.Value.G, c.Value.B);
        }

        private System.Drawing.Color Convert(Color c)
        {
            return System.Drawing.Color.FromArgb(255, c.R, c.G, c.B);

        }

        private void tbColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_initComplete)
                return;

            var textBox = sender as TextBox;
            _ingoreNextUpdate = true;
            SetColor(ParseInput(textBox.Text));
        }

        private void HexOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if(!_initComplete)
                return;

            var textBox  = sender as TextBox;

            if (textBox == null)
                return;

            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            var valid    = true;

            if(string.IsNullOrEmpty(fullText))
                return;

            if(fullText.Length > 0)
            {
                if(fullText[0] == '#')
                {
                    valid &= fullText.Length <= 7;
                    valid &= fullText.Skip(1).All(c => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'));
                }
                else
                {
                    valid &= fullText.All(c => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'));
                    valid &= fullText.Length <= 6;
                }
            }

            e.Handled = !valid;
        }

    }
}
