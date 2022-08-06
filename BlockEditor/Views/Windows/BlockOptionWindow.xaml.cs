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
        private bool _validInput;
        private SimpleBlock _block;
        private readonly Map _map;
        private readonly string _originalBlockOption;

        public BlockOptionWindow(Map map, MyPoint? index)
        {
            InitializeComponent();

            if(map == null || index == null)
                return;

             _validInput = true;
            _map = map;
            Init(index.Value);
            _originalBlockOption = _block.Options;
        }

        private void Init(MyPoint index)
        {
            _block = _map.Blocks.StartBlocks.GetBlock(index);

            if (_block.IsEmpty())
                _block = _map.Blocks.GetBlock(index);

            if (_block.IsEmpty())
            {
                lblBlockName.Text = "None";
                lblPosX.Text = index.X.ToString(_culture);
                lblPosY.Text = index.Y.ToString(_culture);
                optionTextbox.Text = string.Empty;
            }
            else
            {
                BlockImage.Source  = BlockImages.GetImageBlock(BlockImages.BlockSize.Zoom150, _block.ID).Image;
                lblBlockName.Text  = Block.GetBlockName(_block.ID);
                lblPosX.Text       = _block.Position.Value.X.ToString(_culture);
                lblPosY.Text       = _block.Position.Value.Y.ToString(_culture);
                optionTextbox.Text = _block.Options ?? string.Empty;
            }
        }

        public void ShowNextToClick()
        {
            if(!_validInput)
                return;

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

        private void optionTextbox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var text = optionTextbox.Text;

            if (text == null || _block.IsEmpty())
                return;

            var useOption = !string.Equals(text, _originalBlockOption, StringComparison.CurrentCultureIgnoreCase);

            var b = new SimpleBlock(_block.ID, _block.Position.Value, useOption ? text : string.Empty);
            _map.Blocks.Add(b);

        }
    }
}
