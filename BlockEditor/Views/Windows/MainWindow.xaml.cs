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
    }
}
