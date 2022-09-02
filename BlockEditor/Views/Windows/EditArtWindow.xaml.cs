using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Utils;
using LevelModel.Models.Components.Art;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        private const int _regionIndex = 1;


        public EditArtWindow(Map map, MyRegion region)
        {
            _map = map;
            _region = region;

            InitializeComponent();
            Init();

            if (map == null)
                throw new ArgumentException("map");

            OpenWindows.Add(this);
            MyUtils.SetPopUpWindowPosition(this);
        }

        private void Init()
        {
            var mapItem = new ComboBoxItem();
            mapItem.Name = "Map";

            cbSelection.Items.Add(mapItem);

            var regionItem = new ComboBoxItem();
            regionItem.Name = "Selected Region";

            if (HasSelectedRegion())
            {
                cbSelection.Items.Add(regionItem);
                cbSelection.SelectedIndex = _regionIndex;
            }
            else
            {
                cbSelection.SelectedIndex = 0;
            }
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

        private bool HasSelectedRegion()
        {
            return _region != null && _region.IsComplete();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Page2Title.Content = "Delete Art";
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

        private void RemoveArt()
        {
            if (cbTextArt00.IsChecked == true)
                _map.Level.TextArt00.Clear();

            if (cbTextArt0.IsChecked == true)
                _map.Level.TextArt0.Clear();

            if (cbTextArt1.IsChecked == true)
                _map.Level.TextArt1.Clear();

            if (cbTextArt2.IsChecked == true)
                _map.Level.TextArt2.Clear();

            if (cbTextArt3.IsChecked == true)
                _map.Level.TextArt3.Clear();


            if (cbDrawArt00.IsChecked == true)
                _map.Level.DrawArt00.Clear();

            if (cbDrawArt0.IsChecked == true)
                _map.Level.DrawArt0.Clear();

            if (cbDrawArt1.IsChecked == true)
                _map.Level.DrawArt1.Clear();

            if (cbDrawArt2.IsChecked == true)
                _map.Level.DrawArt2.Clear();

            if (cbDrawArt3.IsChecked == true)
                _map.Level.DrawArt3.Clear();
        }

        private void RemoveArt(MyRegion region)
        {
            var textArt00 = _map.Level.TextArt00.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var textArt0 = _map.Level.TextArt0.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var textArt1 = _map.Level.TextArt1.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var textArt2 = _map.Level.TextArt2.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var textArt3 = _map.Level.TextArt3.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));

            var drawArt00 = _map.Level.DrawArt00.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var drawArt0 = _map.Level.DrawArt0.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var drawArt1 = _map.Level.DrawArt1.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var drawArt2 = _map.Level.DrawArt2.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var drawArt3 = _map.Level.DrawArt3.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));

            if (cbTextArt00.IsChecked == true)
                _map.Level.TextArt00.RemoveAll(a => textArt00.Contains(a));
            if (cbTextArt0.IsChecked == true)
                _map.Level.TextArt0.RemoveAll(a => textArt0.Contains(a));
            if (cbTextArt1.IsChecked == true)
                _map.Level.TextArt1.RemoveAll(a => textArt1.Contains(a));
            if (cbTextArt2.IsChecked == true)
                _map.Level.TextArt2.RemoveAll(a => textArt2.Contains(a));
            if (cbTextArt3.IsChecked == true)
                _map.Level.TextArt3.RemoveAll(a => textArt3.Contains(a));

            if (cbDrawArt00.IsChecked == true)
                _map.Level.DrawArt00.RemoveAll(a => drawArt00.Contains(a));
            if (cbDrawArt0.IsChecked == true)
                _map.Level.DrawArt0.RemoveAll(a => drawArt0.Contains(a));
            if (cbDrawArt1.IsChecked == true)
                _map.Level.DrawArt1.RemoveAll(a => drawArt1.Contains(a));
            if (cbDrawArt2.IsChecked == true)
                _map.Level.DrawArt2.RemoveAll(a => drawArt2.Contains(a));
            if (cbDrawArt3.IsChecked == true)
                _map.Level.DrawArt3.RemoveAll(a => drawArt3.Contains(a));
        }

        private void MoveArt(MyRegion region)
        {
            var textArt00 = _map.Level.TextArt00.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var textArt0  = _map.Level.TextArt0.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var textArt1  = _map.Level.TextArt1.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var textArt2  = _map.Level.TextArt2.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var textArt3  = _map.Level.TextArt3.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));

            var drawArt00 = _map.Level.DrawArt00.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var drawArt0  = _map.Level.DrawArt0.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var drawArt1  = _map.Level.DrawArt1.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var drawArt2  = _map.Level.DrawArt2.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var drawArt3  = _map.Level.DrawArt3.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));

            if (cbTextArt00.IsChecked == true)
                MoveTextArt(textArt00);
            if (cbTextArt0.IsChecked == true)
                MoveTextArt(textArt0);
            if (cbTextArt1.IsChecked == true)
                MoveTextArt(textArt1);
            if (cbTextArt2.IsChecked == true)
                MoveTextArt(textArt2);
            if (cbTextArt3.IsChecked == true)
                MoveTextArt(textArt3);

            if (cbDrawArt00.IsChecked == true)
                MoveDrawArt(drawArt00);
            if (cbDrawArt0.IsChecked == true)
                MoveDrawArt(drawArt0);
            if (cbDrawArt1.IsChecked == true)
                MoveDrawArt(drawArt1);
            if (cbDrawArt2.IsChecked == true)
                MoveDrawArt(drawArt2);
            if (cbDrawArt3.IsChecked == true)
                MoveDrawArt(drawArt3);
        }

        private void MoveArt()
        {
            if (cbTextArt00.IsChecked == true)
                MoveTextArt(_map.Level.TextArt00);
            if (cbTextArt0.IsChecked == true)
                MoveTextArt(_map.Level.TextArt0);
            if (cbTextArt1.IsChecked == true)
                MoveTextArt(_map.Level.TextArt1);
            if (cbTextArt2.IsChecked == true)
                MoveTextArt(_map.Level.TextArt2);
            if (cbTextArt3.IsChecked == true)
                MoveTextArt(_map.Level.TextArt3);

            if (cbDrawArt00.IsChecked == true)
                MoveDrawArt(_map.Level.DrawArt00);
            if (cbDrawArt0.IsChecked == true)
                MoveDrawArt(_map.Level.DrawArt0);
            if (cbDrawArt1.IsChecked == true)
                MoveDrawArt(_map.Level.DrawArt1);
            if (cbDrawArt2.IsChecked == true)
                MoveDrawArt(_map.Level.DrawArt2);
            if (cbDrawArt3.IsChecked == true)
                MoveDrawArt(_map.Level.DrawArt3);

        }

        private void MoveTextArt(IEnumerable<Art> art)
        {
            if (art == null)
                return;

            if (art.Count() > 0)
            {
                art.First().X += (int) (_moveX * 30);
                art.First().Y += (int) (_moveY * 30);
            }
        }

        private void MoveDrawArt(IEnumerable<Art> art)
        {
            if (art == null)
                return;

            foreach (Art a in art)
            {
                a.X += (int)(_moveX * 30); ;
                a.Y += (int)(_moveY * 30);
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_moveMode)
                {
                    if (cbSelection.SelectedIndex == _regionIndex)
                    {
                        MoveArt(_region);
                        MessageUtil.ShowInfo("The art inside the selected region has been moved.");
                    }
                    else
                    {
                        MoveArt();
                        MessageUtil.ShowInfo("The art inside the map has been moved.");
                    }
                }
                else
                {
                    if (cbSelection.SelectedIndex == _regionIndex)
                    {
                        RemoveArt(_region);
                        MessageUtil.ShowInfo("The art inside the selected region has been deleted.");
                    }
                    else
                    {
                        RemoveArt();
                        MessageUtil.ShowInfo("The art inside the map has been deleted.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }
    }
}
