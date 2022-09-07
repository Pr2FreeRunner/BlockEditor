using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Views.Controls;
using System.Windows;
using System.Windows.Input;
namespace BlockEditor.Views.Windows
{
    public partial class MenuWindow : Window
    {
        public RelayCommand Option { get; set; }


        public MenuWindow(string title)
        {
            InitializeComponent();
            tbTitle.Content = title;
            OpenWindows.Add(this);
        }

        private bool IsValid(RelayCommand c)
        {
            if (c == null)
                return false;

            if (!c.CanExecute(null))
                return false;

            return true;
        }

        public void AddOption(string text, RelayCommand command)
        {
            if(string.IsNullOrWhiteSpace(text))
                return;

            var b = new WhiteButton(text);
            b.HorizontalAlignment = HorizontalAlignment.Center;
            b.VerticalAlignment = VerticalAlignment.Center;
            b.OnClick += () => { Option = command; Close(); };
            b.Width = 200;
            b.Height = 26;
            b.Margin = new Thickness(10, 10, 10, 10);
            b.IsEnabled = IsValid(command);

            MenuContainer.Children.Add(b);
        }

        public void Execute()
        {
            if (!IsValid(Option))
                return;

            Option.Execute(null);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenWindows.Remove(this);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
                Close();
        }


    }
}
