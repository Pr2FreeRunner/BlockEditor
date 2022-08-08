using BlockEditor.Utils;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlockEditor.Views.Windows
{
    public partial class MapInfoWindow : Window
    {
        private bool _isClosing;

        public MapInfoWindow()
        {
            InitializeComponent();
            MyUtils.SetPopUpWindowPosition(this);
            ItemBlockOptionsControl.OnBlockOptionChanged += OnBlockOptionChanged;
        }

        private void OnBlockOptionChanged(string obj)
        {
        }

        private void Integer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox   = sender as TextBox;
            var fullText  = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            var culture   = CultureInfo.InvariantCulture;
            bool isInteger = !int.TryParse(fullText, NumberStyles.Integer, culture, out var result);

            e.Handled = isInteger && result >= 0;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Time_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CowboyHat_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Title_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Background_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CloseWindow()
        {
            if (_isClosing)
                return;

            _isClosing = true;
            Close();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            CloseWindow();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                CloseWindow();
        }
    }
}
