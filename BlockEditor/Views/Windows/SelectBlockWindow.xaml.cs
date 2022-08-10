using BlockEditor.Models;
using LevelModel.Models.Components;
using System.Windows;
using System.Windows.Input;

namespace BlockEditor.Views.Windows
{
    public partial class SelectBlockWindow : Window
    {

        private SimpleBlock _selectedBlock { get; set; }

        public SelectBlockWindow(string title)
        {
            InitializeComponent();
            tbTitle.Text = title;
            this.Title = "Select Block";
            btnOk.IsEnabled = false;
            MyBlockControl.OnSelectedBlockID += OnBlockSelected;
            OpenWindows.Add(this);
        }

        private void OnBlockSelected(int? b)
        {
            if(b == null)
            {
                _selectedBlock = new SimpleBlock(b.Value);
                btnOk.IsEnabled =  true;
            }
            else
            {
                _selectedBlock = SimpleBlock.None;
                btnOk.IsEnabled = false;
            }
        }

        public static SimpleBlock Show(string title)
        {
            var current = Mouse.OverrideCursor;

            try
            {
                Mouse.OverrideCursor = null;

                var w = new SelectBlockWindow(title);

                w.ShowDialog();

                return w._selectedBlock;
            }
            finally
            {
                Mouse.OverrideCursor = current;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenWindows.Remove(this);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
