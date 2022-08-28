using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlockEditor.Views.Controls
{
    public partial class CustomStatsControl : UserControl
    {

        public event Action<string> OnBlockOptionChanged;
        private const string _reset = "reset";
        private const string _default ="50-50-50";

        public CustomStatsControl()
        {
            InitializeComponent();
        }

        public void SetBlockOptions(string input)
        {
            if (input == null)
                return;

            if(string.Equals(_reset, input, StringComparison.InvariantCultureIgnoreCase))
            {
                cbReset.IsChecked = true;
                speedTb.IsEnabled = false;
                accelTb.IsEnabled = false;
                jumpTb.IsEnabled = false;
                return;
            }

            var split = input.Split("-", StringSplitOptions.RemoveEmptyEntries);

            if(split.Length != 3)
            {
                speedTb.Text = "50";
                accelTb.Text = "50";
                jumpTb.Text  = "50";
                return;
            }

            speedTb.Text = split[0];
            accelTb.Text = split[1];
            jumpTb.Text  = split[2];
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(speedTb.Text) && string.IsNullOrWhiteSpace(accelTb.Text) && string.IsNullOrWhiteSpace(jumpTb.Text))
            {
                OnBlockOptionChanged?.Invoke(string.Empty);
                return;
            }

            if (string.IsNullOrWhiteSpace(speedTb.Text))
                return;

            if (string.IsNullOrWhiteSpace(accelTb.Text))
                return;

            if (string.IsNullOrWhiteSpace(jumpTb.Text))
                return;

            var option = (speedTb.Text + "-" + accelTb.Text + "-" + jumpTb.Text).Trim();

            if(string.Equals(option, _default, StringComparison.InvariantCultureIgnoreCase))
                option = string.Empty;

            OnBlockOptionChanged?.Invoke(option);
        }

        private void TB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            var culture = CultureInfo.InvariantCulture;
            bool isInteger = int.TryParse(fullText, NumberStyles.Integer, culture, out var result);

            e.Handled = !isInteger || result < 0 || result > 100;
        }

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            speedTb.IsEnabled = !(cbReset.IsChecked.HasValue ? cbReset.IsChecked.Value : false);
            accelTb.IsEnabled = !(cbReset.IsChecked.HasValue ? cbReset.IsChecked.Value : false);
            jumpTb.IsEnabled  = !(cbReset.IsChecked.HasValue ? cbReset.IsChecked.Value : false);

            if (cbReset.IsChecked == true)
                OnBlockOptionChanged?.Invoke(_reset);
            else
                TextBox_TextChanged(null, null);
        }
    }
}
