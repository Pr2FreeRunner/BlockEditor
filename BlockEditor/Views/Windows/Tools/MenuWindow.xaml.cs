using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Views.Controls;
using System;
using System.Windows;
using System.Windows.Input;
namespace BlockEditor.Views.Windows
{
    public partial class MenuWindow : ToolWindow
    {
        public RelayCommand Option { get; set; }


        public MenuWindow(string title)
        {
            InitializeComponent();
            tbTitle.Content = title;
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
            b.Width = 210;
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ClickButton(int nr)
        {
            int index = nr - 1;

            if(index < 0)
                return;

            if(index >= MenuContainer.Children.Count)
                return;

            var child = MenuContainer.Children[index];

            if(!(child is WhiteButton b))
                return;

            if(!b.IsEnabled)
                return;

            b.InvokeClickEvent();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
                Close();

            if(e.Key == Key.D1 || e.Key == Key.NumPad1) ClickButton(1);
            if(e.Key == Key.D2 || e.Key == Key.NumPad2) ClickButton(2);
            if(e.Key == Key.D3 || e.Key == Key.NumPad3) ClickButton(3);
            if(e.Key == Key.D4 || e.Key == Key.NumPad4) ClickButton(4);
            if(e.Key == Key.D5 || e.Key == Key.NumPad5) ClickButton(5);
            if(e.Key == Key.D6 || e.Key == Key.NumPad6) ClickButton(6);
            if(e.Key == Key.D7 || e.Key == Key.NumPad7) ClickButton(7);
            if(e.Key == Key.D8 || e.Key == Key.NumPad8) ClickButton(8);
            if(e.Key == Key.D9 || e.Key == Key.NumPad9) ClickButton(9);
        }


    }
}
