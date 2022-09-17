using BlockEditor.Helpers;
using BlockEditor.Models;
using LevelModel.Models.Components;
using System.Windows;
using System.Windows.Input;
using static BlockEditor.Models.BlockImages;

namespace BlockEditor.Views.Windows
{
    public partial class SelectBlockWindow : ToolWindow
    {

        private int? _selectedBlock { get; set; }

        private SelectBlockWindow(string title)
        {
            InitializeComponent();
            MyBlockControl.Init(BlockSize.Zoom100, 11, whiteSelection: false);

            tbTitle.Text = title;
            this.Title = "Select Block";
            MyBlockControl.OnSelectedBlockID += OnBlockSelected;
        }

        private void OnBlockSelected(int? b)
        {
            _selectedBlock = b;
            Close();
        }

        public static int? Show(string title, bool startblocks)
        {
            var current = Mouse.OverrideCursor;

            try
            {
                Mouse.OverrideCursor = null;

                var w = new SelectBlockWindow(title);

                w.ShowDialog();

                if(!startblocks && Block.IsStartBlock(w._selectedBlock))
                {
                    MessageUtil.ShowError("Selecting a start-block is not allowed, redo it!");
                    return Show(title, startblocks);
                }

                return w._selectedBlock;
            }
            finally
            {
                Mouse.OverrideCursor = current;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedBlock = null;
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                _selectedBlock = null;
                Close();
            }
        }
    }
}
