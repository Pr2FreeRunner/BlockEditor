using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlockEditor.Views.Windows
{
    public partial class MovePastedBlocksWindow : ToolWindow
    {
        public int? MoveX;
        public int? MoveY;

        public List<SimpleBlock> BlocksToAdd { get; }
        private readonly List<SimpleBlock> _blocksToRemove;


        public MovePastedBlocksWindow(List<SimpleBlock> blocksToRemove)
        {
            BlocksToAdd = new List<SimpleBlock>();
            _blocksToRemove = blocksToRemove;

            InitializeComponent();
            tbCount.Text = blocksToRemove?.Count.ToString() ?? "";
            UpdateButtons();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void UpdateButtons()
        {
            btnOk.IsEnabled = MoveX != null && MoveY != null;
        }

        private void Integer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            var culture = CultureInfo.InvariantCulture;
            bool isNotInteger = !int.TryParse(fullText, NumberStyles.Any, culture, out var result);

            if (string.Equals(fullText, "-", StringComparison.InvariantCultureIgnoreCase))
                return;

            e.Handled = isNotInteger && result >= 0;
        }

        private void tbY_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = tbY.Text;

            if (MyUtil.TryParse(text, out var result))
                MoveY = result;
            else
                MoveY = null;

            UpdateButtons();
        }

        private void tbX_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = tbX.Text;

            if (MyUtil.TryParse(text, out var result))
                MoveX = result;
            else
                MoveX = null;

            UpdateButtons();
        }
   
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
