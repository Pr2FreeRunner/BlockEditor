using System;
using System.Windows;
using System.Windows.Controls;

namespace BlockEditor.Views.Controls
{
    public partial class WhiteButton : UserControl
    {
        public event Action OnClick;

        public WhiteButton(string text)
        {
            InitializeComponent();
            btn.Content = text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InvokeClickEvent();
        }


        public void InvokeClickEvent()
        {
            OnClick?.Invoke();
        }
    }
}
