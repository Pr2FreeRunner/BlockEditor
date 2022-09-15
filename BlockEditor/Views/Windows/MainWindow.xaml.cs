using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BlockEditor.Models;
using BlockEditor.Views.Controls;
using static BlockEditor.Models.BlockImages;

namespace BlockEditor.Views.Windows
{
    public partial class MainWindow : Window
    {

        private MyTabControl _currentTab;
        private int tabNumber = 1;
        public static readonly DateTime StartTime;

        public MapControl CurrentMap { get; set; }
        public MyTabControl CurrentTab { 
            get 
            { 
                return _currentTab;
            }
            set 
            { 
                if(_currentTab != null)
                    _currentTab.OnSelection(false);

                _currentTab = value;

                MapPanel.Children.Clear();

                if (_currentTab != null)
                {
                    CurrentMap = _currentTab.MapControl;
                    MapPanel.Children.Add(CurrentMap);
                    _currentTab.OnSelection(true);
                }
            }
        }


        static MainWindow()
        {
            StartTime = DateTime.UtcNow;

            MySettings.Init();
            BlockImages.Init();
            UserMode.Init();
        }

        public MainWindow()
        {
            InitializeComponent();
            SetBlockImageSize();
            BlocksControl.OnSelectedBlockID += OnSelectedBlockId;
            BlockSelection.CleanUserBlockControl = BlocksControl.RemoveSelection;
            CreateNewTab();
        }

        private BlockSize GetBlockSize(double height = double.NaN)
        {
            if (double.IsNaN(height))
                height = this.Height;

            if (double.IsNaN(height))
                height = SystemParameters.WorkArea.Size.Height;

            if (double.IsNaN(height))
                return BlockSize.Zoom120;

            if (height < 740)
                return BlockSize.Zoom80;
            if(height < 820)
                return BlockSize.Zoom100;
            if(height < 890)
                return BlockSize.Zoom120;
            if (height < 990)
                return BlockSize.Zoom140;
            if (height < 1200)
                return BlockSize.Zoom160;

            return BlockSize.Zoom180;
        }


        private void SetBlockImageSize(double height = double.NaN)
        {
            var blockSize = GetBlockSize(height);
            BlocksControl.Init(blockSize, 3);
        }

        private void OnSelectedBlockId(int? id)
        {
            if(CurrentMap == null)
                return;

            BlockSelection.SelectedBlocks = null;
            BlockSelection.SelectedBlock = id;

            foreach (var child in TabPanel.Children)
            {
                if(child is MyTabControl tab && tab != null)
                    tab.MapControl.ViewModel.Game.Mode.Value = UserMode.UserModes.None;
            }
        }

        public void ResetUserMode()
        {
            BlockSelection.Reset();

            if(CurrentMap == null)
                return;

            CurrentMap.ViewModel.Game.Mode.Value = UserMode.UserModes.None;
        }

        public void TitleChanged(string title)
        {
            if(CurrentTab == null)
                return;

            if(string.IsNullOrEmpty(title))
                return;

            CurrentTab.tbTitle.Text = title;
        }

        private void CreateNewTab()
        {
            var tab = new MyTabControl(tabNumber);

            tab.OnClick += Tab_OnClick;
            tab.OnClose += Tab_OnClose;
            CurrentTab   = tab;

            TabPanel.Children.Insert(TabPanel.Children.Count - 1, tab);
            tabNumber++;
        }

        private void Tab_OnClick(MyTabControl tab)
        {
            if(tab == null)
                return;

            CurrentTab = tab;
        }

        private void Tab_OnClose(MyTabControl tab)
        {
            if(tab == null)
                return;

            TabPanel.Children.Remove(tab);
            var count = TabPanel.Children.Count;

            if (count > 1)
                CurrentTab = (TabPanel.Children[TabPanel.Children.Count - 2] as MyTabControl);
            else
                Close();
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if(CurrentMap == null)
                return;

            CurrentMap.ViewModel.Game.Engine.Pause = false;
            base.OnPreviewGotKeyboardFocus(e);
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (CurrentMap == null)
                return;

            CurrentMap.ViewModel.Game.Engine.Pause = true;
            base.OnLostKeyboardFocus(e);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            OpenWindows.ShowAll();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (CurrentMap == null)
                return;

            BlocksControl.OnKeyDown(e.Key);
            CurrentMap.UserControl_PreviewKeyDown(sender, e);
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (CurrentMap == null)
                return;

            CurrentMap.UserControl_PreviewMouseWheel(sender, e);
        }

        private void NewTab_Click(object sender, RoutedEventArgs e)
        {
            CreateNewTab();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetBlockImageSize(e.NewSize.Height);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var endTime = DateTime.UtcNow;
            var timeDiff = endTime - StartTime;

            MySettings.PlayTime += (int)Math.Round(timeDiff.TotalMinutes);
        }

    }
}
