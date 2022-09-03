using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Utils;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace BlockEditor.Views.Windows
{
    public partial class ConnectTeleportsWindow : Window
    {
        public string Option { get; set; }
        public bool AddMore { get; set; }

        public ConnectTeleportsWindow(int count)
        {
            InitializeComponent();

            Init(count);
            OpenWindows.Add(this);
        }

      
        private void Init(int count)
        {
            var culture = CultureInfo.InvariantCulture;
            tbCount.Content = "Selected Block Count:  " + count;

            MyColorPicker.SetColor(string.Empty);
            MyColorPicker.OnNewColor += MyColorPicker_OnNewColor;
        }

        private void MyColorPicker_OnNewColor(string text)
        {
            try
            {
                if(!string.IsNullOrEmpty(text))
                    text = Convert.ToInt32(text, 16).ToString();
            }
            catch
            {
                MessageUtil.ShowError("Failed to convert color to PR2 block option format.");
            }

            Option = text;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenWindows.Remove(this);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void btnAddMore_Click(object sender, RoutedEventArgs e)
        {
            AddMore = true;
            DialogResult = true;
        }
    }
}
