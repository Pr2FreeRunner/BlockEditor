using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Utils;
using LevelModel.Models.Components;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using static BlockEditor.Models.UserMode;

namespace BlockEditor.Views.Controls
{
    public partial class ConnectTeleportsControl : UserControl
    {

        private readonly ConnectTeleports _data;
        private readonly Game _game;
        private readonly Cursor _connectCursor;
        private readonly List<SimpleBlock> _allTeleports;

        public ConnectTeleportsControl(Game game)
        {
            InitializeComponent();
            _game = game;
            _data = new ConnectTeleports();
            _connectCursor = Mouse.OverrideCursor;
            _allTeleports = GetAllTeleportBlocks();
            AutoPairCheckbox.IsChecked = MySettings.AutoConnectPair;
            Init();
        }

        private List<SimpleBlock> GetAllTeleportBlocks()
        {
            return _game.Map.Blocks.GetBlocks(false).Where(b => !b.IsEmpty() && b.ID == Block.TELEPORT).ToList();
        }

        private void Init()
        {
            var culture = CultureInfo.InvariantCulture;
            var color   =  MySettings.LastTeleportColor ?? string.Empty;

            MyColorPicker.SetColor(color);
            MyColorPicker.OnNewColor += MyColorPicker_OnNewColor;

            _data.Start();
            _data.Options = color;
            UpdateGui();
        }

        private void UpdateGui()
        {
            tbCount.Content = "Selected Block Count:  " + _data.Count;
            btnOK.IsEnabled = _data.Count > 0;
            btnReset.IsEnabled = _data.Count > 0 || !string.IsNullOrEmpty(_data.Options);
            tbUnique.Content = "Is Color Unique:  " + (AutoPairCheckbox.IsChecked == true ? "Auto" : IsColorUnique() ? "Yes" : "No");
            AutoPairCheckbox.IsEnabled = AutoPairCheckbox.IsChecked == true || _data.Count == 0;
        }

        public void Add(SimpleBlock point)
        {
            _data.Add(point);

            if(_data.Count == 2 && AutoPairCheckbox.IsChecked == true)
            {
                while(!IsColorUnique() || string.IsNullOrWhiteSpace(_data.Options))
                    AssignNextColor();

                AssignTeleports();
                _data.ClearSelectedBlocks();
            }

            UpdateGui(); 
        }

        public void AssignNextColor()
        {
            var option = _data.Options;
            var fallback = "0";

            if (string.IsNullOrWhiteSpace(option))
            {
                MyColorPicker.SetColor(fallback);
                return;
            }

            if(!MyUtil.TryParse(option, out var value))
            {
                MyColorPicker.SetColor(fallback);
                return;
            }

            value++;
            MyColorPicker.SetColor("#" + value.ToString("X6"));
        }

        private void AssignTeleports()
        {
            using (new TempCursor(Cursors.Wait))
            {
                var blocks = _data.GetAddedBlocks();
                _allTeleports.AddRange(blocks);
                _game.AddBlocks(blocks);
            }
        }

        private void Close(bool addBlocks)
        {
            App.MyMainWindow?.CurrentMap?.ClearSidePanel();

            if(_game == null)
                return;

            _game.Mode.Value = UserModes.None;

            if(addBlocks)
                AssignTeleports();
        }


        private void MyColorPicker_OnNewColor(string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                    text = Convert.ToInt32(text, 16).ToString();
            }
            catch
            {
                MessageUtil.ShowError("Failed to convert color to PR2 block option format.");
            }

            _data.Options = text;
            MySettings.LastTeleportColor = text;
            UpdateGui();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close(false);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Close(true);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close(false);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            _data.Start();
            MyColorPicker.SetColor(string.Empty);
            UpdateGui();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = null;
            }
            catch { }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                if(_game.Mode.Value == UserModes.ConnectTeleports)
                    Mouse.OverrideCursor = _connectCursor;
            }
            catch {}
        }

        private bool IsColorUnique()
        {
            foreach (var b in _allTeleports)
            {
                if(_data.IsSelected(b.Position))
                    continue;

                if (string.Equals(_data.Options, b.Options, StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }

            return true;
        }

        private void AutoPairCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            MySettings.AutoConnectPair = AutoPairCheckbox.IsChecked ?? false;
            UpdateGui();
        }
    }
}
