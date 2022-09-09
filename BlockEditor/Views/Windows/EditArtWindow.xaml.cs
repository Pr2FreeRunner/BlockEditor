﻿using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Utils;
using LevelModel.Models.Components.Art;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Builders.DataStructures.DTO.ImageDTO;

namespace BlockEditor.Views.Windows
{
    public partial class EditArtWindow : Window
    {
        private Map _map;
        private MyRegion _region;

        private double? _moveX;
        private double? _moveY;
        private string _colorReplace;
        private string _colorAdd;
        private ColorSensitivty? _sensitivity;

        private EditArtModes _mode;
        private const int _regionIndex = 1;

        public List<SimpleBlock> BlocksToAdd { get; }
        public List<SimpleBlock> BlocksToRemove { get; }

        public enum EditArtModes { Move, Delete, ReplaceColor, ReverseTraps }

        public EditArtWindow(Map map, MyRegion region, EditArtModes mode)
        {
            _map = map;
            _region = region;
            BlocksToAdd = new List<SimpleBlock>();
            BlocksToRemove = new List<SimpleBlock>();
            _mode = mode;

            InitializeComponent();

            if (map == null)
                throw new ArgumentException("map");

            Init();
            UpdateButtons();
            OpenWindows.Add(this);
        }

        private void MyColorPickerReplace_OnNewColor(string obj)
        {
            _colorReplace = obj;
            UpdateButtons();
        }

        private void MyColorPickerAdd_OnNewColor(string obj)
        {
            _colorAdd = obj;
            UpdateButtons();
        }

        private void Init()
        {
            MyColorPickerAdd.OnNewColor += MyColorPickerAdd_OnNewColor;
            MyColorPickerReplace.OnNewColor += MyColorPickerReplace_OnNewColor;

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

            foreach (var s in Enum.GetValues(typeof(ColorSensitivty)))
            { 
                var item = new ComboBoxItem();
                item.Content = MyUtil.InsertSpaceBeforeCapitalLetter(s.ToString());
                cbSensitivity.Items.Add(item);
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

        private bool HasSelectedRegion()
        {
            return _region != null && _region.IsComplete();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void UpdateButtons()
        {
            switch (_mode)
            {
                case EditArtModes.Move:
                    btnOk.IsEnabled = _moveX != null && _moveY != null;
                    Page2Title.Content = "Move";
                    MovePanel.Visibility = Visibility.Visible;
                    ReplaceColorPanel.Visibility = Visibility.Collapsed;
                    cbBlocks.Visibility = Visibility.Visible;
                    break;
                case EditArtModes.Delete:
                    btnOk.IsEnabled = true;
                    Page2Title.Content = "Delete";
                    MovePanel.Visibility = Visibility.Collapsed;
                    ReplaceColorPanel.Visibility = Visibility.Collapsed;
                    cbBlocks.Visibility = Visibility.Visible;
                    break;
                case EditArtModes.ReplaceColor:
                    btnOk.IsEnabled = _colorAdd != null && _colorReplace != null && _sensitivity != null;
                    Page2Title.Content = "Replace Art Color";
                    MovePanel.Visibility = Visibility.Collapsed;
                    ReplaceColorPanel.Visibility = Visibility.Visible;
                    cbBlocks.Visibility = Visibility.Collapsed;
                    break;
                case EditArtModes.ReverseTraps:
                    btnOk.IsEnabled = true;
                    Page2Title.Content = "Reverse";
                    MovePanel.Visibility = Visibility.Collapsed;
                    ReplaceColorPanel.Visibility = Visibility.Collapsed;
                    cbBlocks.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Double_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            var culture = CultureInfo.InvariantCulture;
            bool isNotInteger = !double.TryParse(fullText, NumberStyles.Any, culture, out var result);

            if (string.Equals(fullText, "-", StringComparison.InvariantCultureIgnoreCase))
                return;

            e.Handled = isNotInteger && result >= 0;
        }

        private void tbY_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = tbY.Text;

            if (MyUtil.TryParseDouble(text, out var result))
                _moveY = result;

            UpdateButtons();
        }

        private void tbX_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = tbX.Text;

            if (MyUtil.TryParseDouble(text, out var result))
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
            if (arts == null)
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

            var drawArt0 = _map.Level.DrawArt0.Where(a => region.IsInside(a));
            var drawArt1 = _map.Level.DrawArt1.Where(a => region.IsInside(a));

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
            var x = (int)(_moveX * 30);
            var y = (int)(_moveY * 30);

            CreateAbsolutePosition(_map.Level.TextArt0);
            CreateAbsolutePosition(_map.Level.TextArt1);


            var textArt0 = _map.Level.TextArt0.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var textArt1 = _map.Level.TextArt1.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));

            var drawArt0 = _map.Level.DrawArt0.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var drawArt1 = _map.Level.DrawArt1.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));

            if (cbTextArt0.IsChecked == true)
                MapUtil.MoveAbsoluteArt(textArt0, x, y);
            if (cbTextArt1.IsChecked == true)
                MapUtil.MoveAbsoluteArt(textArt1, x, y);

            if (cbDrawArt0.IsChecked == true)
                MapUtil.MoveAbsoluteArt(drawArt0, x, y);
            if (cbDrawArt1.IsChecked == true)
                MapUtil.MoveAbsoluteArt(drawArt1, x, y);

            CreateRelativePosition(_map.Level.TextArt0);
            CreateRelativePosition(_map.Level.TextArt1);
        }

        private void ReverseArt(MyRegion region)
        {
            CreateAbsolutePosition(_map.Level.TextArt0);
            CreateAbsolutePosition(_map.Level.TextArt1);


            var textArt0 = _map.Level.TextArt0.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var textArt1 = _map.Level.TextArt1.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));

            var drawArt0 = _map.Level.DrawArt0.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var drawArt1 = _map.Level.DrawArt1.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));

            if (cbTextArt0.IsChecked == true)
                MapUtil.ReverseArtPosition(textArt0);
            if (cbTextArt1.IsChecked == true)
                MapUtil.ReverseArtPosition(textArt1);

            if (cbDrawArt0.IsChecked == true)
                MapUtil.ReverseArtPosition(drawArt0);
            if (cbDrawArt1.IsChecked == true)
                MapUtil.ReverseArtPosition(drawArt1);

            CreateRelativePosition(_map.Level.TextArt0);
            CreateRelativePosition(_map.Level.TextArt1);
        }

        private void ReverseArt()
        {
            CreateAbsolutePosition(_map.Level.TextArt0);
            CreateAbsolutePosition(_map.Level.TextArt1);

            if (cbTextArt0.IsChecked == true)
                MapUtil.ReverseArtPosition(_map.Level.TextArt0);
            if (cbTextArt1.IsChecked == true)
                MapUtil.ReverseArtPosition(_map.Level.TextArt1);

            if (cbDrawArt0.IsChecked == true)
                MapUtil.ReverseArtPosition(_map.Level.DrawArt0);
            if (cbDrawArt1.IsChecked == true)
                MapUtil.ReverseArtPosition(_map.Level.DrawArt1);

            CreateRelativePosition(_map.Level.TextArt0);
            CreateRelativePosition(_map.Level.TextArt1);
        }

        private void ReverseBlocks(MyRegion region)
        {
            if (cbBlocks.IsChecked != true)
                return;

            foreach (var b in BlocksUtil.GetBlocks(_map?.Blocks, region))
            {
                if (b.IsEmpty())
                    continue;

                var point = new MyPoint(Blocks.SIZE - b.Position.Value.X, b.Position.Value.Y);
                var block = new SimpleBlock(b.ID, point, b.Options);

                BlocksToRemove.Add(b);
                BlocksToAdd.Add(block);
            }
        }

        private void ReverseBlocks()
        {
            if (cbBlocks.IsChecked != true)
                return;

            if (_map == null)
                return;

            foreach (var b in _map?.Blocks.GetBlocks(true))
            {
                if (b.IsEmpty())
                    continue;

                var point = new MyPoint(Blocks.SIZE - b.Position.Value.X, b.Position.Value.Y);
                var block = new SimpleBlock(b.ID, point, b.Options);

                BlocksToRemove.Add(b);
                BlocksToAdd.Add(block);
            }
        }

        private void MoveArt()
        {
            var x = (int)(_moveX * 30);
            var y = (int)(_moveY * 30);

            if (cbTextArt0.IsChecked == true)
                MapUtil.MoveRelativeArt(_map.Level.TextArt0, x, y);
            if (cbTextArt1.IsChecked == true)
                MapUtil.MoveRelativeArt(_map.Level.TextArt1, x, y);

            if (cbDrawArt0.IsChecked == true)
                MapUtil.MoveAbsoluteArt(_map.Level.DrawArt0, x, y);
            if (cbDrawArt1.IsChecked == true)
                MapUtil.MoveAbsoluteArt(_map.Level.DrawArt1, x, y);
        }

        private void MoveBlocks(MyRegion region = null)
        {
            if (cbBlocks.IsChecked != true)
                return;

            if (_moveX == null)
                return;

            if (_moveY == null)
                return;

            foreach(var b in BlocksUtil.GetBlocks(_map?.Blocks, region))
            {
                if(b.IsEmpty())
                    continue;

                var point = new MyPoint(b.Position.Value.X + (int)_moveX, b.Position.Value.Y + (int)_moveY);
                var block = new SimpleBlock(b.ID, point, b.Options);

                BlocksToRemove.Add(b);
                BlocksToAdd.Add(block);
            }
        }

        private void RemoveBlocks(MyRegion region = null)
        {
            if (cbBlocks.IsChecked != true)
                return;

            foreach (var b in BlocksUtil.GetBlocks(_map?.Blocks, region))
            {
                if (b.IsEmpty())
                    continue;

                BlocksToRemove.Add(b);
            }
        }

        private void ReplaceArtColor(MyRegion region)
        {
            var replace = ColorUtil.ToSkColor(_colorReplace);
            var add     = ColorUtil.ToSkColor(_colorAdd);

            CreateAbsolutePosition(_map.Level.TextArt0);
            CreateAbsolutePosition(_map.Level.TextArt1);


            var textArt0 = _map.Level.TextArt0.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var textArt1 = _map.Level.TextArt1.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));

            var drawArt0 = _map.Level.DrawArt0.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var drawArt1 = _map.Level.DrawArt1.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));

            if (cbTextArt0.IsChecked == true)
                MapUtil.ChangeArtColor(textArt0, replace, add, _sensitivity.Value);
            if (cbTextArt1.IsChecked == true)
                MapUtil.ChangeArtColor(textArt1, replace, add, _sensitivity.Value);

            if (cbDrawArt0.IsChecked == true)
                MapUtil.ChangeArtColor(drawArt0, replace, add, _sensitivity.Value);
            if (cbDrawArt1.IsChecked == true)
                MapUtil.ChangeArtColor(drawArt1, replace, add, _sensitivity.Value);

            CreateRelativePosition(_map.Level.TextArt0);
            CreateRelativePosition(_map.Level.TextArt1);
        }

        private void ReplaceArtColor()
        {
            var replace = ColorUtil.ToSkColor(_colorReplace);
            var add = ColorUtil.ToSkColor(_colorAdd);

            CreateAbsolutePosition(_map.Level.TextArt0);
            CreateAbsolutePosition(_map.Level.TextArt1);

            if (cbTextArt0.IsChecked == true)
                MapUtil.ChangeArtColor(_map.Level.TextArt0, replace, add, _sensitivity.Value);
            if (cbTextArt1.IsChecked == true)
                MapUtil.ChangeArtColor(_map.Level.TextArt1, replace, add, _sensitivity.Value);

            if (cbDrawArt0.IsChecked == true)
                MapUtil.ChangeArtColor(_map.Level.DrawArt0, replace, add, _sensitivity.Value);
            if (cbDrawArt1.IsChecked == true)
                MapUtil.ChangeArtColor(_map.Level.DrawArt1, replace, add, _sensitivity.Value);

            CreateRelativePosition(_map.Level.TextArt0);
            CreateRelativePosition(_map.Level.TextArt1);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using(new TempCursor(Cursors.Wait))
                {
                    switch (_mode)
                    {
                        case EditArtModes.Move:
                            if (cbSelection.SelectedIndex == _regionIndex)
                            {
                                MoveArt(_region);
                                MoveBlocks(_region);
                            }
                            else
                            {
                                MoveArt();
                                MoveBlocks();
                            }
                            break;
                        case EditArtModes.Delete:
                            if (cbSelection.SelectedIndex == _regionIndex)
                            {
                                RemoveArt(_region);
                                RemoveBlocks(_region);
                            }
                            else
                            {
                                RemoveArt();
                                RemoveBlocks();
                            }
                            break;
                        case EditArtModes.ReplaceColor:
                            if (cbSelection.SelectedIndex == _regionIndex)
                            {
                                ReplaceArtColor(_region);
                            }
                            else
                            {
                                ReplaceArtColor();
                            }
                            break;
                        case EditArtModes.ReverseTraps:
                            if (cbSelection.SelectedIndex == _regionIndex)
                            {
                                ReverseArt(_region);
                                ReverseBlocks(_region);
                            }
                            else
                            {
                                ReverseArt();
                                ReverseBlocks();
                            }
                            break;
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

        private void cbSensitivity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbSensitivity.SelectedIndex != -1)
                    _sensitivity = (ColorSensitivty)(cbSensitivity.SelectedIndex + 1);
                else
                    _sensitivity = null;

                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }
    }
}
