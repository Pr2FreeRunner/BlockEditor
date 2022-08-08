using System;
using System.Windows;
using System.Globalization;
using System.Windows.Input;
using BlockEditor.Models;
using LevelModel.Models.Components;
using BlockEditor.Utils;
using BlockEditor.Views.Controls;

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

            if (map == null || index == null)
                return;

            MyUtils.SetPopUpWindowPosition(this);
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
            }
            else
            {
                BlockImage.Source  = BlockImages.GetImageBlock(BlockImages.BlockSize.Zoom150, _block.ID).Image;
                lblBlockName.Text  = Block.GetBlockName(_block.ID);
                lblPosX.Text       = _block.Position.Value.X.ToString(_culture);
                lblPosY.Text       = _block.Position.Value.Y.ToString(_culture);

                if(_block.ID == Block.ITEM_BLUE || _block.ID == Block.ITEM_RED)
                {
                    var c = new ItemBlockOptionsControl();
                    c.Margin = new Thickness(5,0,10,20);
                    c.OnBlockOptionChanged += OnOptionsChanged;
                    OptionPanel.Children.Add(c);
                }
                else
                {
                    var c = new BlockOptionsControl();
                    c.Margin = new Thickness(10, 0, 10, 20);
                    c.OnBlockOptionChanged += OnOptionsChanged;
                    OptionPanel.Children.Add(c);
                }
            }
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

        private void OnOptionsChanged(string text)
        {
            if (text == null || _block.IsEmpty())
                return;

            var useOption = !string.Equals(text, _originalBlockOption, StringComparison.CurrentCultureIgnoreCase);

            var b = new SimpleBlock(_block.ID, _block.Position.Value, useOption ? text : string.Empty);
            _map.Blocks.Add(b);

        }
    }
}
