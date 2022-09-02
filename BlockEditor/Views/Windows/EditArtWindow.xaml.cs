﻿using BlockEditor.Helpers;
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

        public string Message { get; set; }

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
            mapItem.Content = "Map";

            cbSelection.Items.Add(mapItem);

            var regionItem = new ComboBoxItem();
            regionItem.Content = "Selected Region";

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
            if (cbTextArt0.IsChecked == true)
                _map.Level.TextArt0.Clear();

            if (cbTextArt1.IsChecked == true)
                _map.Level.TextArt1.Clear();



            if (cbDrawArt0.IsChecked == true)
                _map.Level.DrawArt0.Clear();

            if (cbDrawArt1.IsChecked == true)
                _map.Level.DrawArt1.Clear();
        }

        private void CreateAbsolutePosition(List<TextArt> arts)
        {
            if(arts == null)
                return;

            var x = 0;
            var y = 0;

            foreach (var a in arts)
            {
                x += a.X;
                y += a.Y;

                a.X = x;
                a.Y = y;
            }
        }

        private void CreateRelativePosition(List<TextArt> arts)
        {
            if (arts == null)
                return;

            var previousX = 0;
            var previousY = 0;

            foreach (var a in arts)
            {
                var tempX = a.X;
                var tempY = a.Y;

                a.X -= previousX;
                a.Y -= previousY;

                previousX = tempX;
                previousY = tempY;
            }
        }

        private void RemoveArt(MyRegion region)
        {
            CreateAbsolutePosition(_map.Level.TextArt0);
            CreateAbsolutePosition(_map.Level.TextArt1);

            var textArt0 = _map.Level.TextArt0.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var textArt1 = _map.Level.TextArt1.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));

            var drawArt0 = _map.Level.DrawArt0.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var drawArt1 = _map.Level.DrawArt1.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));

            if (cbTextArt0.IsChecked == true)
                _map.Level.TextArt0.RemoveAll(a => textArt0.Contains(a));
            if (cbTextArt1.IsChecked == true)
                _map.Level.TextArt1.RemoveAll(a => textArt1.Contains(a));

            if (cbDrawArt0.IsChecked == true)
                _map.Level.DrawArt0.RemoveAll(a => drawArt0.Contains(a));
            if (cbDrawArt1.IsChecked == true)
                _map.Level.DrawArt1.RemoveAll(a => drawArt1.Contains(a));

            CreateRelativePosition(_map.Level.TextArt0);
            CreateRelativePosition(_map.Level.TextArt1);
        }

        private void MoveArt(MyRegion region)
        {
            CreateAbsolutePosition(_map.Level.TextArt0);
            CreateAbsolutePosition(_map.Level.TextArt1);

            var textArt0  = _map.Level.TextArt0.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var textArt1  = _map.Level.TextArt1.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));

            var drawArt0  = _map.Level.DrawArt0.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var drawArt1  = _map.Level.DrawArt1.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));

            if (cbTextArt0.IsChecked == true)
                MoveAbsoluteArt(textArt0);
            if (cbTextArt1.IsChecked == true)
                MoveAbsoluteArt(textArt1);

            if (cbDrawArt0.IsChecked == true)
                MoveAbsoluteArt(drawArt0);
            if (cbDrawArt1.IsChecked == true)
                MoveAbsoluteArt(drawArt1);

            CreateRelativePosition(_map.Level.TextArt0);
            CreateRelativePosition(_map.Level.TextArt1);
        }

        private void MoveArt()
        {
            if (cbTextArt0.IsChecked == true)
                MoveRelativeArt(_map.Level.TextArt0);
            if (cbTextArt1.IsChecked == true)
                MoveRelativeArt(_map.Level.TextArt1);

            if (cbDrawArt0.IsChecked == true)
                MoveAbsoluteArt(_map.Level.DrawArt0);
            if (cbDrawArt1.IsChecked == true)
                MoveAbsoluteArt(_map.Level.DrawArt1);
        }

        private void MoveRelativeArt(IEnumerable<Art> art)
        {
            if (art == null)
                return;

            if (art.Count() > 0)
            {
                art.First().X += (int) (_moveX * 30);
                art.First().Y += (int) (_moveY * 30);
            }
        }

        private void MoveAbsoluteArt(IEnumerable<Art> art)
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
                        Message = "The art inside the selected region has been moved.";
                    }
                    else
                    {
                        MoveArt();
                        Message = "The art inside the map has been moved.";
                    }
                }
                else
                {
                    if (cbSelection.SelectedIndex == _regionIndex)
                    {
                        RemoveArt(_region);
                        Message = "The art inside the selected region has been deleted.";
                    }
                    else
                    {
                        RemoveArt();
                        Message = "The art inside the map has been deleted.";
                    }
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}