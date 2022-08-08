using BlockEditor.Models;
using Microsoft.Win32;
using System;
using System.Windows;


namespace BlockEditor.Views.Windows
{

    public partial class WindowSaveMap : Window
    {
        public string MapTitle { get; set; }

        public bool Publish { get; set; }

        public string LocalFilepath { get; set; }

        public WindowSaveMap(Map map)
        {
            if(map == null)
                throw new ArgumentNullException("Map");

            InitializeComponent();
            txtResponse.Text = MapTitle = map.Backend.Title;
            btnPr2.IsEnabled = CurrentUser.IsLoggedIn();

            OpenWindows.Add(this);
        }

        private void btnPr2_Click(object sender, RoutedEventArgs e)
        {
            startGrid.Visibility = Visibility.Collapsed;
            saveLevelGrid.Visibility = Visibility.Visible;

            UpdateButtons();
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

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            Publish = publishCheckBox.IsChecked.HasValue ? publishCheckBox.IsChecked.Value : false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenWindows.Remove(this);
        }
    }
}
