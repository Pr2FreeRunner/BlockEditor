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
        private bool _isClosing;
        private Map _map;

        public MapInfoWindow(Map map)
        {
            _map = map;
            InitializeComponent();

            if(map == null)
                throw new ArgumentException("map");

            MyUtils.SetPopUpWindowPosition(this);
            Init();
            ItemBlockOptionsControl.OnItemChanged += OnItemBlockOptionChanged;
        }

        private void Init()
        {
            tbtTitle.Text = _map.Backend.Title;
            tbTime.Text = _map.Backend.MaxTime.ToString(CultureInfo.InvariantCulture);
            tbCowboy.Text = _map.Backend.CowboyChance.ToString(CultureInfo.InvariantCulture);
            ItemBlockOptionsControl.SetItems(_map.Backend.Items);
        }

        private void OnItemBlockOptionChanged(List<Item> items)
        {
            if(items == null)
                return;

            _map.Backend.Items = items;
        }

        private void Integer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox   = sender as TextBox;
            var fullText  = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            var culture   = CultureInfo.InvariantCulture;
            bool isInteger = !int.TryParse(fullText, NumberStyles.Integer, culture, out var result);

            e.Handled = isInteger && result >= 0;
        }

        private void Time_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (MyUtils.TryParse(tb.Text, out var result))
                _map.Backend.MaxTime = result;
        }

        private void CowboyHat_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if(tb == null)
                return;

            if(MyUtils.TryParse(tb.Text, out var result))
                _map.Backend.CowboyChance = result;
        }

        private void Title_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            _map.Backend.Title = tb.Text;
        }

        private void Background_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            //_map.Backend.BackgroundColor = tb.Text;
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
