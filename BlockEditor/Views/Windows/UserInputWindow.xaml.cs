using BlockEditor.Models;
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
using System.Windows.Shapes;

namespace BlockEditor.Views.Windows
{

    public partial class UserInputWindow : Window
    {


        public UserInputWindow(string question, string title, string defaultValue = "")
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PromptDialog_Loaded);
            txtQuestion.Text = question;
            Title = title;
            txtResponse.Text = defaultValue;
            UpdateButtons();

            OpenWindows.Add(this);
        }

        void PromptDialog_Loaded(object sender, RoutedEventArgs e)
        {
            txtResponse.Focus();
        }

        public static string Show(string question, string title, string defaultValue = "")
        {
            var current = Mouse.OverrideCursor;

            try 
            { 
                Mouse.OverrideCursor = null;

                var inst = new UserInputWindow(question, title, defaultValue);

                inst.ShowDialog();

                if (inst.DialogResult == true)
                    return inst.ResponseText;

                return string.Empty;
            }
            finally
            {
                Mouse.OverrideCursor = current;
            }
        }

        public string ResponseText
        {
            get
            {
                return txtResponse.Text;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UpdateButtons()
        {
            btnOk.IsEnabled = !string.IsNullOrWhiteSpace(txtResponse.Text);
        }
        private void txtResponse_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateButtons();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenWindows.Remove(this);
        }
    }
}
