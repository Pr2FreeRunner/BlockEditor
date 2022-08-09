using BlockEditor.Models;
using BlockEditor.Utils;
using LevelModel.Models.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlockEditor.Views.Windows
{
    public partial class MapInfoWindow : Window
    {
        private Map _map;



        public MapInfoWindow(Map map)
        {
            _map = map;
            InitializeComponent();

            if(map == null)
                throw new ArgumentException("map");

            OpenWindows.Add(this);
            MyUtils.SetPopUpWindowPosition(this);
            Init();
            ItemBlockOptionsControl.OnItemChanged += OnItemBlockOptionChanged;
            HatsControl.OnHatChanged += OnHatChanged;
            MyColorPicker.SetColor(map.Background);
            MyColorPicker.OnNewColor += OnNewColor;
        }

        private void OnNewColor(string color)
        {
            if(string.IsNullOrWhiteSpace(color))
                return;

            _map.Level.BackgroundColor = color;
        }

        private void OnHatChanged(List<int> hats)
        {
            if(hats == null)
                return;

            _map.Level.BadHats = hats;
        }

        private void Init()
        {
            tbtTitle.Text = _map.Level.Title;
            tbTime.Text = _map.Level.MaxTime.ToString(CultureInfo.InvariantCulture);
            tbCowboy.Text = _map.Level.CowboyChance.ToString(CultureInfo.InvariantCulture);
            tbRank.Text = _map.Level.RankLimit.ToString(CultureInfo.InvariantCulture);
            tbGravity.Text = _map.Level.Gravity.ToString(CultureInfo.InvariantCulture);
            tbMode.Text = _map.Level.GameMode?.FullName ?? string.Empty;

            ItemBlockOptionsControl.SetItems(_map.Level.Items);
        }

        private void OnItemBlockOptionChanged(List<Item> items)
        {
            if(items == null)
                return;

            _map.Level.Items = items;
        }

        private void Integer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox    = sender as TextBox;
            var fullText   = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            var culture    = CultureInfo.InvariantCulture;
            bool isInteger = int.TryParse(fullText, NumberStyles.Integer, culture, out var result);

            e.Handled = !isInteger || result < 0;
        }

        private void IntegerMax100_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            var culture = CultureInfo.InvariantCulture;
            bool isInteger = int.TryParse(fullText, NumberStyles.Integer, culture, out var result);

            e.Handled = !isInteger || result < 0 || result > 100;
        }


        private void Double_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            var culture = CultureInfo.InvariantCulture;
            bool isInteger = !double.TryParse(fullText, NumberStyles.Any, culture, out var result);

            e.Handled = isInteger && result >= 0;
        }

        private void Time_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (MyUtils.TryParse(tb.Text, out var result))
                _map.Level.MaxTime = result;
        }

        private void CowboyHat_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if(tb == null)
                return;

            if(MyUtils.TryParse(tb.Text, out var result))
                _map.Level.CowboyChance = result;
        }

        private void Title_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            _map.Level.Title = tb.Text;
        }

        private void Background_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            //_map.Backend.BackgroundColor = tb.Text;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void Rank_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (MyUtils.TryParse(tb.Text, out var result))
                _map.Level.RankLimit = result;
        }

        private void Gravity_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (MyUtils.TryParseDouble(tb.Text, out var result))
                _map.Level.Gravity = result;
        }

        private void Mode_TextChanged(object sender, SelectionChangedEventArgs e)
        {
            var text = ((sender as ComboBox)?.SelectedItem as ComboBoxItem)?.Content as string;

            if (text == null)
                return;

            var g = new GameMode(text);

            if (string.IsNullOrWhiteSpace(g.FullName))
                return;

            _map.Level.GameMode = g;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenWindows.Remove(this);
        }
    }
}
