using BlockEditor.Models;
using DataAccess;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlockEditor.Views.Windows
{

    public partial class LoginWindow : Window
    {

        private string UserName { get; set; }
        private string Password { get; set; }

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

        private bool IsOK()
        {
            return !string.IsNullOrWhiteSpace(UserName)
               && !string.IsNullOrWhiteSpace(Password);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            using(new TempCursor(Cursors.Wait))
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NameTextbox.Focus();
        }
    }
}
