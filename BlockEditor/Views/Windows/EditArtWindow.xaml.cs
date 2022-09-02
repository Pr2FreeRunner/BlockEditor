using BlockEditor.Models;
using BlockEditor.Utils;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlockEditor.Views.Windows
{
    public partial class EditArtWindow : Window
    {
        private Map _map;
        private MyRegion _region;

        private double? _moveX;
        private double? _moveY;
        private bool _moveMode = false;

        public EditArtWindow(Map map, MyRegion region)
        {
            _map = map;
            _region = region;

            InitializeComponent();

            if(map == null)
                throw new ArgumentException("map");

            OpenWindows.Add(this);
            MyUtils.SetPopUpWindowPosition(this);
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenWindows.Remove(this);
        }


        private void btnMoveArt_Click(object sender, RoutedEventArgs e)
        {
            Page2Title.Content = "Move Art";
            Page1.Visibility = Visibility.Collapsed;
            Page2.Visibility = Visibility.Visible;
            MovePanel.Visibility = Visibility.Visible;
            btnOk.IsEnabled = false;
            _moveMode = true;

            UpdateButtons();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Page2Title.Content = "Remove Art";
            Page1.Visibility = Visibility.Collapsed;
            Page2.Visibility = Visibility.Visible;
            MovePanel.Visibility = Visibility.Collapsed;
            _moveMode = false;

            UpdateButtons();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void UpdateButtons()
        {
            btnOk.IsEnabled = !_moveMode || (_moveMode && _moveX != null && _moveY != null);
        }

        private void Double_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            var culture = CultureInfo.InvariantCulture;
            bool isInteger = !double.TryParse(fullText, NumberStyles.Any, culture, out var result);

            e.Handled = isInteger && result >= 0;
        }

        private void tbY_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = tbY.Text;

            if (MyUtils.TryParseDouble(text, out var result))
                _moveY = result;

            UpdateButtons();
        }

        private void tbX_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = tbX.Text;

            if (MyUtils.TryParseDouble(text, out var result))
                _moveX = result;

            UpdateButtons();

        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if(_moveMode)
            {

            }
            else
            {

            }
        }
    }
}
