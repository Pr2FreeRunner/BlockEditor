using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlockEditor.Views.Controls
{
    public partial class StatsBlockControl : UserControl
    {

        public event Action<string> OnBlockOptionChanged;
        private const string _default ="5";
        private bool _isSadBlock;

        public StatsBlockControl(bool isSadBlock)
        {
            InitializeComponent();
            _isSadBlock = isSadBlock;
        }

        public void SetBlockOptions(string input)
        {
            if (input == null)
                return;

            if(input.StartsWith('-'))
                input = input.Substring(1);

            textbox.Text = string.IsNullOrWhiteSpace(input) ? "5" : input;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var option = textbox?.Text != null ? textbox.Text.Trim() : string.Empty;

            if (string.Equals(option, _default, StringComparison.InvariantCultureIgnoreCase))
                option = string.Empty;

            if(_isSadBlock && !string.IsNullOrWhiteSpace(option))
                option = "-" + option;

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

    }
}
