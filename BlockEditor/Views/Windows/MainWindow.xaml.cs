using BlockEditor.Models;
using System;
using System.Windows;
using System.Windows.Input;

namespace BlockEditor.Views.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        static MainWindow()
        {
            MySettings.Init();
            BlockImages.Init();
            UserMode.Init();
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            MyMapControl.ViewModel.Game.Engine.Pause = false;
            base.OnPreviewGotKeyboardFocus(e);
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            MyMapControl.ViewModel.Game.Engine.Pause = true;
            base.OnLostKeyboardFocus(e);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            OpenWindows.ShowAll();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            MyMapControl.UserControl_PreviewKeyDown(sender, e);
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            MyMapControl.UserControl_PreviewMouseWheel(sender, e);
        }
    }
}
