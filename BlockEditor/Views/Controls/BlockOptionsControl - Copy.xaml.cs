using System;
using System.Windows.Controls;

namespace BlockEditor.Views.Controls
{
    public partial class BlockOptionsControl : UserControl
    {

        public event Action<string> OnBlockOptionChanged;

        public BlockOptionsControl()
        {
            InitializeComponent();
        }

        public void SetBlockOptions(string input)
        {
            if (input == null)
                return;

            textbox.Text = input;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnBlockOptionChanged?.Invoke(textbox.Text);
        }
    }
}
