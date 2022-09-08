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
        private bool _moveMode;
        private const int _regionIndex = 1;

        public List<SimpleBlock> BlocksToAdd { get; }
        public List<SimpleBlock> BlocksToRemove { get; }


        public EditArtWindow(Map map, MyRegion region, bool move)
        { 
            _map = map;
            _region = region;
            BlocksToAdd = new List<SimpleBlock>();
            BlocksToRemove = new List<SimpleBlock>();
            _moveMode = move;

            InitializeComponent();
            Init();

            if (map == null)
                throw new ArgumentException("map");

            OpenWindows.Add(this);
            UpdateButtons();
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
            btnOk.IsEnabled = !_moveMode || (_moveMode && _moveX != null && _moveY != null);
            MovePanel.Visibility = _moveMode ? Visibility.Visible : Visibility.Collapsed;
            Page2Title.Content = _moveMode ? "Move" : "Delete";
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

            foreach(var b in MapUtil.GetBlocks(_map, region))
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

            foreach (var b in MapUtil.GetBlocks(_map, region))
            {
                if (b.IsEmpty())
                    continue;

                BlocksToRemove.Add(b);
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using(new TempCursor(Cursors.Wait))
                {
                    if (_moveMode)
                    {
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
                    }
                    else
                    {
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
