using BlockEditor.Models;
using BlockEditor.Views.Controls;
using System;
using System.Windows;
using System.Windows.Input;
using System.Linq;

namespace BlockEditor.Views.Windows
{
    public partial class MainWindow : Window
    {

        private MapControl _currentMap;
        private MyTabControl _currentTab;
        private int tabNumber = 1;

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
                    _currentMap = _currentTab.MapControl;
                    MapPanel.Children.Add(_currentMap);
                    _currentTab.OnSelection(true);
                }
            }
        }


        static MainWindow()
        {
            MySettings.Init();
            BlockImages.Init();
            UserMode.Init();
        }

        public MainWindow()
        {
            InitializeComponent();
            CreateNewTab();
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
            if(_currentMap == null)
                return;

            _currentMap.ViewModel.Game.Engine.Pause = false;
            base.OnPreviewGotKeyboardFocus(e);
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (_currentMap == null)
                return;

            _currentMap.ViewModel.Game.Engine.Pause = true;
            base.OnLostKeyboardFocus(e);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            OpenWindows.ShowAll();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_currentMap == null)
                return;

            _currentMap.UserControl_PreviewKeyDown(sender, e);
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_currentMap == null)
                return;

            _currentMap.UserControl_PreviewMouseWheel(sender, e);
        }

        private void NewTab_Click(object sender, RoutedEventArgs e)
        {
            CreateNewTab();
        }
    }
}
