using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace BlockEditor.Models
{
    public class ToolWindow : Window
    {

        public ToolWindow()
        {
            this.MouseLeftButtonDown += ToolWindow_MouseLeftButtonDown;
            this.Closing += ToolWindow_Closing;

            OpenWindows.Add(this);
        }

        private void ToolWindow_Closing(object sender, CancelEventArgs e)
        {
            OpenWindows.Remove(this);
        }

        public void ToolWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}
