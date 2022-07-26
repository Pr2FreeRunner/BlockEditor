using BlockEditor.Models;
using DataAccess;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlockEditor.Views
{

    public partial class LoginWindow : Window
    {

        private string UserName { get; set; }
        private string Password { get; set; }

        private bool IsOK()
        {
            return !string.IsNullOrWhiteSpace(UserName)
               && !string.IsNullOrWhiteSpace(Password);

        }

        public LoginWindow()
        {
            InitializeComponent();
            UpdateButton();
        }

        private void UpdateButton()
        {
            BtnOk.IsEnabled = IsOK();
            ErrorTextbox.Text = string.Empty;

            CurrentUserPanel.Visibility = CurrentUser.IsLoggedIn() ? Visibility.Visible : Visibility.Collapsed;
            CurrentUserTextbox.Text     = CurrentUser.IsLoggedIn() ? CurrentUser.Name   : string.Empty;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                var success = CurrentUser.Login(UserName, Password, out var errorMsg);

                UpdateButton();

                if (success)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    ErrorTextbox.Text = errorMsg;
                }

            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Password_TextChanged(object sender, TextChangedEventArgs e)
        {
            Password = PasswordTextbox.Text;
            UpdateButton();
        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserName = NameTextbox.Text;
            UpdateButton();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.Logout();
            UpdateButton();
        }
    }
}
