using BlockEditor.Models;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;

namespace BlockEditor.Views.Windows
{

    public partial class SaveMapWindow : Window
    {
        public string MapTitle { get; set; }

        public bool Publish { get; set; }
        public bool Newest { get; set; }


        public string LocalFilepath { get; set; }

        public SaveMapWindow(Map map)
        {
            if(map == null)
                throw new ArgumentNullException("Map");

            InitializeComponent();
            txtResponse.Text = MapTitle = map.Level.Title;
            btnPr2.IsEnabled = Users.IsLoggedIn();
            publishCheckBox.IsChecked = map.Level.Published;

            if (btnPr2.IsEnabled)
                btnPr2.Focus();

            OpenWindows.Add(this);
        }

        private void btnPr2_Click(object sender, RoutedEventArgs e)
        {
            startGrid.Visibility = Visibility.Collapsed;
            saveLevelGrid.Visibility = Visibility.Visible;

            UpdateButtons();
            txtResponse.Focus();

            if(!string.IsNullOrEmpty(txtResponse.Text))
                txtResponse.CaretIndex = txtResponse.Text.Length;
        }

        private void btnLocalFile_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog() { Filter = "Text file (*.txt)|*.txt" } ;

            if (saveFileDialog.ShowDialog() == true)
            {
                LocalFilepath = saveFileDialog.FileName;
                DialogResult = true;
                Close();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void UpdateButtons()
        {
            btnSave.IsEnabled = !string.IsNullOrWhiteSpace(MapTitle);
        }

        private void txtResponse_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            MapTitle = txtResponse.Text;
            UpdateButtons();
        }

        private void PublishCheckboxChanged(object sender, RoutedEventArgs e)
        {
            Publish = publishCheckBox.IsChecked.HasValue ? publishCheckBox.IsChecked.Value : false;

            newestCheckBox.IsEnabled = Publish;
            newestCheckBox.IsChecked = Publish;
        }

        private void NewestCheckboxChanged(object sender, RoutedEventArgs e)
        {
            Newest = newestCheckBox.IsChecked.HasValue ? newestCheckBox.IsChecked.Value : false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenWindows.Remove(this);
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
                Close();

            if (e.Key == Key.Enter && saveLevelGrid.Visibility == Visibility.Visible && btnSave.IsEnabled)
                btnSave_Click(null, null);
        }
    }
}
