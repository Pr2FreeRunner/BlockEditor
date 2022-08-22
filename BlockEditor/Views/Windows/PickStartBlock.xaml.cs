using BlockEditor.Models;
using LevelModel.Models.Components;
using System.Windows;
using System.Windows.Input;
using static BlockEditor.Utils.ShapeBuilderUtil;

namespace BlockEditor.Views.Windows
{

    public partial class PickStartBlock : Window
    {
        public int? Result { get; private set; }

        public PickStartBlock()
        {
            InitializeComponent();
            OpenWindows.Add(this);
        }


        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenWindows.Remove(this);
        }

        private void btnPlayer1_Click(object sender, RoutedEventArgs e)
        {
            Result = Block.START_BLOCK_P1;
            DialogResult = true;
            Close();
        }
        private void btnPlayer2_Click(object sender, RoutedEventArgs e)
        {
            Result = Block.START_BLOCK_P2;
            DialogResult = true;
            Close();
        }
        private void btnPlayer3_Click(object sender, RoutedEventArgs e)
        {
            Result = Block.START_BLOCK_P3;
            DialogResult = true;
            Close();
        }
        private void btnPlayer4_Click(object sender, RoutedEventArgs e)
        {
            Result = Block.START_BLOCK_P4;
            DialogResult = true;
            Close();
        }
    }
}
