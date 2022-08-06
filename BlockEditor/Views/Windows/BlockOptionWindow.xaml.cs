using System;
using System.Windows;
using System.Globalization;
using System.Windows.Input;
using BlockEditor.Models;
using LevelModel.Models.Components;

namespace BlockEditor.Views.Windows
{
    public partial class BlockOptionWindow : Window
    {

        private readonly CultureInfo _culture = CultureInfo.InvariantCulture;
        private bool isClosing;

        public BlockOptionWindow(SimpleBlock block, MyPoint? index = null)
        {
            InitializeComponent();

            if (block.IsEmpty())
            {
                lblBlockName.Text = "None";
                lblPosX.Text = index.HasValue ? index.Value.X.ToString(_culture) : "";
                lblPosY.Text = index.HasValue ? index.Value.Y.ToString(_culture) : "";
                optionTextbox.Text = string.Empty;
            }
            else
            {
                BlockImage.Source = BlockImages.GetImageBlock(BlockImages.BlockSize.Zoom150, block.ID).Image;
                lblBlockName.Text = Block.GetBlockName(block.ID);
                lblPosX.Text = block.Position.Value.X.ToString(_culture);
                lblPosY.Text = block.Position.Value.Y.ToString(_culture);
                optionTextbox.Text = block.Options ?? string.Empty;
            }
        }

        public void ShowNextToClick()
        {
            var topControlHeight = 100;
            var mainWindow = App.Current.MainWindow;
            var startPos   = mainWindow.PointToScreen(Mouse.GetPosition(mainWindow));

            var marginX    = 50;
            var marginY    = 150;

            var height = double.IsNaN(this.Height) ? this.MinHeight : this.Height;
            var width  = double.IsNaN(this.Width)  ? this.MinWidth : this.Width;

            var posX = startPos.X - marginX - width;
            var posY = startPos.Y - marginY;


            var underflowX = posX - mainWindow.Left;
            var underflowY = posY - mainWindow.Top - topControlHeight;

            if (underflowX < 0)
                posX -= underflowX - marginX;

            if(underflowY < 0)
                posY -= underflowY - marginY;

            var overflowX = posX + width  - mainWindow.Width;
            var overflowY = posY + height - mainWindow.Height;

            if (overflowX > 0)
                posX -= overflowX + marginX;

            if (overflowY > 0)
                posY -= overflowY + marginY;

            this.Left = posX;
            this.Top  = posY;
            Show();
        }

        private void CloseWindow()
        {
            if(isClosing)
                return;

            isClosing = true;
            Close();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            CloseWindow();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
                CloseWindow();
        }
    }
}
