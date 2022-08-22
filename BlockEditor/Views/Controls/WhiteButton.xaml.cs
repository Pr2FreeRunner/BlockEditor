using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlockEditor.Views.Controls
{
    /// <summary>
    /// Interaction logic for WhiteButton.xaml
    /// </summary>
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
            OnClick?.Invoke();
        }
    }
}
