using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Utils;
using LevelModel.Models.Components.Art;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Builders.DataStructures.DTO.ImageDTO;
using static LevelModel.Models.Level;

namespace BlockEditor.Views.Windows
{
    public partial class EditRegionWindow : ToolWindow
    {
        private Game _game;
        private MyRegion _region;

        private double? _moveX;
        private double? _moveY;
        private string _colorReplace;
        private string _colorAdd;
        private ColorSensitivty? _sensitivity;
        private const int _regionIndex = 1;
        private EditArtModes _mode;

        private readonly List<SimpleBlock> _blocksToAdd;
        private readonly List<SimpleBlock> _blocksToRemove;

        public enum EditArtModes { Move, Delete, ReplaceColor }


        public EditRegionWindow(Game game, MyRegion region, EditArtModes mode)
        {
            _game = game;
            _region = region;
            _blocksToAdd = new List<SimpleBlock>();
            _blocksToRemove = new List<SimpleBlock>();
            _mode = mode;

            InitializeComponent();

            if (game?.Map == null)
                throw new ArgumentException("map");

            Init();
            UpdateButtons();
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

        private bool HasSelectedRegion()
        {
            return _region.IsComplete();
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
                    cbTextArt00.Visibility = cbTextArt2.Visibility = cbTextArt2.Visibility = cbTextArt3.Visibility = Visibility.Collapsed;
                    cbDrawArt00.Visibility = cbDrawArt2.Visibility = cbDrawArt2.Visibility = cbDrawArt3.Visibility = Visibility.Collapsed;
                    break;
                case EditArtModes.Delete:
                    btnOk.IsEnabled = true;
                    Page2Title.Content = "Delete";
                    MovePanel.Visibility = Visibility.Collapsed;
                    ReplaceColorPanel.Visibility = Visibility.Collapsed;
                    cbBlocks.Visibility = Visibility.Visible;
                    cbTextArt00.Visibility = cbTextArt2.Visibility = cbTextArt2.Visibility = cbTextArt3.Visibility = IsRegionSelected() ? Visibility.Collapsed : Visibility.Visible;
                    cbDrawArt00.Visibility = cbDrawArt2.Visibility = cbDrawArt2.Visibility = cbDrawArt3.Visibility = IsRegionSelected() ? Visibility.Collapsed : Visibility.Visible;
                    break;
                case EditArtModes.ReplaceColor:
                    btnOk.IsEnabled = _colorAdd != null && _colorReplace != null && _sensitivity != null;
                    Page2Title.Content = "Replace Art Color";
                    MovePanel.Visibility = Visibility.Collapsed;
                    ReplaceColorPanel.Visibility = Visibility.Visible;
                    cbBlocks.Visibility = Visibility.Collapsed;
                                        cbTextArt00.Visibility = cbTextArt2.Visibility = cbTextArt2.Visibility = cbTextArt3.Visibility = Visibility.Collapsed;
                    cbDrawArt00.Visibility = cbDrawArt2.Visibility = cbDrawArt2.Visibility = cbDrawArt3.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void Double_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Double_PreviewTextInput(sender, e, null, null);
        }

        private void tbY_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = tbY.Text;

            if (MyUtil.TryParseDouble(text, out var result))
                _moveY = result;
            else
                _moveY = null;

            UpdateButtons();
        }

        private void tbX_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = tbX.Text;

            if (MyUtil.TryParseDouble(text, out var result))
                _moveX = result;
            else
                _moveX = null;

            UpdateButtons();

        }

        private void RemoveDrawArt(List<DrawArt> art)
        {
            if(art == null)
                return;

            art.Clear();
        }

        private void RemoveTextArt(List<TextArt> art)
        {
            if (art == null)
                return;

            art.Clear();
        }

        private void RemoveArt()
        {
            if (cbTextArt00.IsChecked == true)
                _game.EditTextArt(RemoveTextArt, ArtType.TextArt00);

            if (cbTextArt0.IsChecked == true)
                _game.EditTextArt(RemoveTextArt, ArtType.TextArt0);

            if (cbTextArt1.IsChecked == true)
                _game.EditTextArt(RemoveTextArt, ArtType.TextArt1);

            if (cbTextArt2.IsChecked == true)
                _game.EditTextArt(RemoveTextArt, ArtType.TextArt2);

            if (cbTextArt3.IsChecked == true)
                _game.EditTextArt(RemoveTextArt, ArtType.TextArt3);


            if (cbDrawArt00.IsChecked == true)
                _game.EditDrawArt(RemoveDrawArt, ArtType.DrawArt00);

            if (cbDrawArt0.IsChecked == true)
                _game.EditDrawArt(RemoveDrawArt, ArtType.DrawArt0);

            if (cbDrawArt1.IsChecked == true)
                _game.EditDrawArt(RemoveDrawArt, ArtType.DrawArt1);

            if (cbDrawArt2.IsChecked == true)
                _game.EditDrawArt(RemoveDrawArt, ArtType.DrawArt2);

            if (cbDrawArt3.IsChecked == true)
                _game.EditDrawArt(RemoveDrawArt, ArtType.DrawArt3);
        }

        private void RemoveArt(MyRegion region)
        {
            var textArt0 = _game.Map.Level.TextArt0.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30))).ToList();
            var textArt1 = _game.Map.Level.TextArt1.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30))).ToList();

            var drawArt0 = ArtUtil.GetArtInside(_game.Map.Level.DrawArt0, region); 
            var drawArt1 = ArtUtil.GetArtInside(_game.Map.Level.DrawArt1, region);

            if (cbTextArt0.IsChecked == true && textArt0.Any())
                _game.EditTextArt((a) => a.RemoveAll(a => textArt0.Contains(a)), ArtType.TextArt0);

            if (cbTextArt1.IsChecked == true && textArt1.Any())
                _game.EditTextArt((a) => a.RemoveAll(a => textArt1.Contains(a)), ArtType.TextArt1);

            if (cbDrawArt0.IsChecked == true && drawArt0.Any())
                _game.EditDrawArt((a) => a.RemoveAll(a => drawArt0.Contains(a)), ArtType.DrawArt0);

            if (cbDrawArt1.IsChecked == true&& drawArt1.Any())
                _game.EditDrawArt((a) => a.RemoveAll(a => drawArt1.Contains(a)), ArtType.DrawArt1);
        }

        private void MoveArt(MyRegion region)
        {
            var x = (int)(_moveX * 30);
            var y = (int)(_moveY * 30);

            var textArt0 = _game.Map.Level.TextArt0.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var textArt1 = _game.Map.Level.TextArt1.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));

            var drawArt0 = ArtUtil.GetArtInside(_game.Map.Level.DrawArt0, region);
            var drawArt1 = ArtUtil.GetArtInside(_game.Map.Level.DrawArt1, region);

            if (cbTextArt0.IsChecked == true)
                MapUtil.MoveArt(textArt0, x, y);
            if (cbTextArt1.IsChecked == true)
                MapUtil.MoveArt(textArt1, x, y);

            if (cbDrawArt0.IsChecked == true)
                MapUtil.MoveArt(drawArt0, x, y);
            if (cbDrawArt1.IsChecked == true)
                MapUtil.MoveArt(drawArt1, x, y);
        }

        private void MoveArt()
        {
            var x = (int)(_moveX * 30);
            var y = (int)(_moveY * 30);

            if (cbTextArt0.IsChecked == true)
                MapUtil.MoveArt(_game.Map.Level.TextArt0, x, y);
            if (cbTextArt1.IsChecked == true)
                MapUtil.MoveArt(_game.Map.Level.TextArt1, x, y);

            if (cbDrawArt0.IsChecked == true)
                MapUtil.MoveArt(_game.Map.Level.DrawArt0, x, y);
            if (cbDrawArt1.IsChecked == true)
                MapUtil.MoveArt(_game.Map.Level.DrawArt1, x, y);
        }

        private void MoveBlocks(MyRegion region = null)
        {
            if (cbBlocks.IsChecked != true)
                return;

            if (_moveX == null)
                return;

            if (_moveY == null)
                return;

            foreach(var b in BlocksUtil.GetBlocks(_game.Map?.Blocks, region))
            {
                if(b.IsEmpty())
                    continue;

                var point = new MyPoint(b.Position.Value.X + (int)_moveX, b.Position.Value.Y + (int)_moveY);
                var block = new SimpleBlock(b.ID, point, b.Options);

                _blocksToRemove.Add(b);
                _blocksToAdd.Add(block);
            }
        }

        private void RemoveBlocks(MyRegion region = null)
        {
            if (cbBlocks.IsChecked != true)
                return;

            foreach (var b in BlocksUtil.GetBlocks(_game.Map?.Blocks, region))
            {
                if (b.IsEmpty())
                    continue;

                _blocksToRemove.Add(b);
            }
        }

        private void ReplaceArtColor(MyRegion region)
        {
            var replace = ColorUtil.ToSkColor(ColorUtil.GetColorFromHex(_colorReplace));
            var add     = ColorUtil.ToSkColor(ColorUtil.GetColorFromHex(_colorAdd));

            var textArt0 = _game.Map.Level.TextArt0.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));
            var textArt1 = _game.Map.Level.TextArt1.Where(a => region.IsInside(new MyPoint(a.X / 30, a.Y / 30)));

            var drawArt0 = ArtUtil.GetArtInside(_game.Map.Level.DrawArt0, region);
            var drawArt1 = ArtUtil.GetArtInside(_game.Map.Level.DrawArt1, region);

            if (cbTextArt0.IsChecked == true)
                MapUtil.ChangeArtColor(textArt0, replace, add, _sensitivity.Value, false);
            if (cbTextArt1.IsChecked == true)
                MapUtil.ChangeArtColor(textArt1, replace, add, _sensitivity.Value, false);

            if (cbDrawArt0.IsChecked == true)
                MapUtil.ChangeArtColor(drawArt0, replace, add, _sensitivity.Value, true);
            if (cbDrawArt1.IsChecked == true)
                MapUtil.ChangeArtColor(drawArt1, replace, add, _sensitivity.Value, true);
        }

        private void ReplaceArtColor()
        {
            var replace = ColorUtil.ToSkColor(ColorUtil.GetColorFromHex(_colorReplace));
            var add = ColorUtil.ToSkColor(ColorUtil.GetColorFromHex(_colorAdd));

            if (cbTextArt0.IsChecked == true)
                MapUtil.ChangeArtColor(_game.Map.Level.TextArt0, replace, add, _sensitivity.Value, false);
            if (cbTextArt1.IsChecked == true)
                MapUtil.ChangeArtColor(_game.Map.Level.TextArt1, replace, add, _sensitivity.Value, false);

            if (cbDrawArt0.IsChecked == true)
                MapUtil.ChangeArtColor(_game.Map.Level.DrawArt0, replace, add, _sensitivity.Value, true);
            if (cbDrawArt1.IsChecked == true)
                MapUtil.ChangeArtColor(_game.Map.Level.DrawArt1, replace, add, _sensitivity.Value, true);
        }

        public bool IsRegionSelected()
        {
            return cbSelection.SelectedIndex == _regionIndex;
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
                            if (IsRegionSelected())
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
                            if (IsRegionSelected())
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
                            if (IsRegionSelected())
                            {
                                ReplaceArtColor(_region);
                            }
                            else
                            {
                                ReplaceArtColor();
                            }
                            break;
                    }

                    if (_blocksToRemove.AnyBlocks())
                        _game.RemoveBlocks(_blocksToRemove);

                    if (_blocksToAdd.AnyBlocks())
                        _game.AddBlocks(_blocksToAdd);

                    _game.Map.LoadArt();
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

        private void cbSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtons();
        }
    }
}
