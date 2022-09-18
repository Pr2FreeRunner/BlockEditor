using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Utils;
using DataAccess;
using LevelModel.Models;
using LevelModel.Models.Components;
using LevelModel.Models.Components.Art;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlockEditor.Views.Windows
{
    public partial class MapInfoWindow : ToolWindow
    {
        private Map _map;
        private Action _refreshGui;
        private int _page = 1;

        public MapInfoWindow(Map map, Action refreshGui)
        {
            _map = map;
            _refreshGui = refreshGui;
            InitializeComponent();

            if(map == null)
                throw new ArgumentException("map");

            Init();
            UpdateButtons();
        }

        private void OnNewColor(string color)
        {
            if(string.IsNullOrWhiteSpace(color))
                return;

            _map.Level.BackgroundColor = color;
            _refreshGui?.Invoke();
        }

        private void OnHatChanged(List<int> hats)
        {
            if(hats == null)
                return;

            _map.Level.BadHats = hats;
        }

        private void Init()
        {
            var culture = CultureInfo.InvariantCulture;
            var drawArt = GetDrawArtSize();
            var textArt = GetTextArtSize();

            tbId.Text = _map.Level.LevelID != default(int) ? _map.Level.LevelID.ToString(culture) : string.Empty;
            tbVersion.Text = _map.Level.Version.ToString(culture);
            tbtTitle.Text = _map.Level.Title ?? string.Empty;
            tbTime.Text = _map.Level.MaxTime.ToString(culture);
            tbCowboy.Text = _map.Level.CowboyChance.ToString(culture);
            tbRank.Text = _map.Level.RankLimit.ToString(culture);
            tbGravity.Text = _map.Level.Gravity.ToString(culture);
            tbUserId.Text = _map.Level.UserID != 0 ? _map.Level.UserID.ToString(culture) : string.Empty;
            tbMode.Text = _map.Level.GameMode?.FullName ?? string.Empty;
            tbDrawArt.Text = drawArt.ToString(culture);
            tbTextArt.Text = textArt.ToString(culture);
            tbNote.Text = _map.Level.Note ?? string.Empty;

            btnUser.IsEnabled = !string.IsNullOrWhiteSpace(tbUserId.Text);
            btnDrawArt.IsEnabled = drawArt != 0;
            btnTextArt.IsEnabled = textArt != 0;

            ItemBlockOptionsControl.SetItems(_map.Level.Items);
            HatsControl.SetBadHats(_map.Level.BadHats);
            MyColorPicker.SetColor(_map.Background.ToString());

            ItemBlockOptionsControl.OnItemChanged += OnItemBlockOptionChanged;
            HatsControl.OnHatChanged += OnHatChanged;
            MyColorPicker.OnNewColor += OnNewColor;
        }

        private void OnItemBlockOptionChanged(List<Item> items)
        {
            if(items == null)
                return;

            _map.Level.Items = items;
        }

        private void Integer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Integer_PreviewTextInput(sender, e, 0, null);
        }

        private void IntegerMax100_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Integer_PreviewTextInput(sender, e, 0, 100);
        }

        private void Double_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Double_PreviewTextInput(sender, e, null, null);
        }

        private void Time_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (MyUtil.TryParse(tb.Text, out var result))
                _map.Level.MaxTime = result;
        }

        private void CowboyHat_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if(tb == null)
                return;

            if(MyUtil.TryParse(tb.Text, out var result))
                _map.Level.CowboyChance = result;
        }

        private void Title_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            _map.Level.Title = tb.Text;
            App.MyMainWindow?.TitleChanged(_map.Level.Title);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();

            var ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            if(ctrl && e.Key == Key.Left && btnLeftPage.IsEnabled)
                OnPreviousPage(null, null);

            if (ctrl && e.Key == Key.Right && btnRightPage.IsEnabled)
                OnNextPage(null, null);
        }

        private void Rank_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (MyUtil.TryParse(tb.Text, out var result))
                _map.Level.RankLimit = result;
        }

        private void Gravity_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (MyUtil.TryParseDouble(tb.Text, out var result))
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

        private void UpdateButtons()
        {
            btnRightPage.IsEnabled = _page < 3;
            btnLeftPage.IsEnabled  = _page > 1;
            PageText.Text = _page.ToString(CultureInfo.InvariantCulture);

            Page1.Visibility = _page == 1 ? Visibility.Visible : Visibility.Collapsed;
            Page2.Visibility = _page == 2 ? Visibility.Visible : Visibility.Collapsed;
            Page3.Visibility = _page == 3 ? Visibility.Visible : Visibility.Collapsed;

        }

        private int GetSize(string s)
        {
            return Encoding.UTF8.GetByteCount(s);
        }

        private int GetDrawArtSize()
        {
            return GetSize(_map.Level.DrawArt00.ToPr2String())
                 + GetSize(_map.Level.DrawArt0.ToPr2String())
                 + GetSize(_map.Level.DrawArt1.ToPr2String())
                 + GetSize(_map.Level.DrawArt2.ToPr2String())
                 + GetSize(_map.Level.DrawArt3.ToPr2String());
        }

        private int GetTextArtSize()
        {
            return GetSize(_map.Level.TextArt00.ToPr2String())
                 + GetSize(_map.Level.TextArt0.ToPr2String())
                 + GetSize(_map.Level.TextArt1.ToPr2String())
                 + GetSize(_map.Level.TextArt2.ToPr2String())
                 + GetSize(_map.Level.TextArt3.ToPr2String());
        }

        private void OnPreviousPage(object sender, RoutedEventArgs e)
        {
            try
            {
                _page--;

                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }

        }

        private void OnNextPage(object sender, RoutedEventArgs e)
        {
            try
            {
                _page++;

                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        private void RemoveDrawArt_Click(object sender, RoutedEventArgs e)
        {
            var result = UserQuestionWindow.Show("Do you wish to delete all Draw-Art?", "Delete", false);

            if(result != UserQuestionWindow.QuestionResult.Yes)
                return;

            _map.Level.DrawArt00?.Clear();
            _map.Level.DrawArt0?.Clear();
            _map.Level.DrawArt1?.Clear();
            _map.Level.DrawArt2?.Clear();
            _map.Level.DrawArt3?.Clear();
            _map.LoadArt();
            _refreshGui();

            tbDrawArt.Text = GetDrawArtSize().ToString(CultureInfo.InvariantCulture);
            UpdateButtons();
        }

        private void RemoveTextArt_Click(object sender, RoutedEventArgs e)
        {
            var result = UserQuestionWindow.Show("Do you wish to delete all Text-Art?", "Delete", false);

            if (result != UserQuestionWindow.QuestionResult.Yes)
                return;

            _map.Level.TextArt00?.Clear();
            _map.Level.TextArt0?.Clear();
            _map.Level.TextArt1?.Clear();
            _map.Level.TextArt2?.Clear();
            _map.Level.TextArt3?.Clear();
            _map.LoadArt();
            _refreshGui();

            tbTextArt.Text = GetTextArtSize().ToString(CultureInfo.InvariantCulture);
            UpdateButtons();
        }

        private void Note_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            _map.Level.Note = tb.Text;
        }

        private void GetUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var info = PR2Accessor.GetUser((uint)_map.Level.UserID);

                if(string.IsNullOrEmpty(info?.Name))
                {
                    MessageUtil.ShowInfo("User not found...");
                    return;
                }

                MessageUtil.ShowInfo("The user is:  " + info.Name);
            }
            catch(Exception ex)
            {
                if (!MyUtil.HasInternet())
                    MessageUtil.ShowError("Failed to get username, check ur internet connection...");
                else
                    MessageUtil.ShowError(ex.Message);
            }
        }
    }
}
