using System;
using System.Windows;
using System.Windows.Input;

namespace BlockEditor.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            Map.ViewModel.Engine.Pause = false;
            base.OnPreviewGotKeyboardFocus(e);
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            Map.ViewModel.Engine.Pause = true;
            base.OnLostKeyboardFocus(e);
        }

    }
}
