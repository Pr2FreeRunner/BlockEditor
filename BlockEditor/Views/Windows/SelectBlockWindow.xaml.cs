using BlockEditor.Helpers;
using BlockEditor.Models;
using LevelModel.Models.Components;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static BlockEditor.Models.BlockImages;

namespace BlockEditor.Views.Windows
{
    public partial class SelectBlockWindow : Window
    {

        private List<int> _selectedBlocks { get; set; }

        private SelectBlockWindow(string title)
        {
            InitializeComponent();
            MyBlockControl.Init(BlockSize.Zoom75, 11, whiteSelection: false);

            tbTitle.Text = title;
            this.Title = "Select Block";
            MyBlockControl.OnSelectedBlockID += OnBlocksSelected;
            OpenWindows.Add(this);
        }

        private void OnBlocksSelected(List<int> blocks, bool ctrl)
        {
            _selectedBlocks = blocks;

            if(!ctrl)
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

                if(!startblocks && w._selectedBlocks.Any(b => Block.IsStartBlock(b)))
                {
                    MessageUtil.ShowError("Selecting a start-block is not allowed, redo it!");
                    return Show(title, startblocks);
                }

                return w._selectedBlocks.First();
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedBlocks = null;
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                _selectedBlocks = null;
                Close();
            }
        }
    }
}
