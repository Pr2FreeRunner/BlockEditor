using System;
using System.Windows;
using System.Globalization;
using System.Windows.Input;
using BlockEditor.Models;
using LevelModel.Models.Components;
using BlockEditor.Utils;
using BlockEditor.Views.Controls;
using System.Linq;
using System.Windows.Controls;
using BlockEditor.Helpers;

namespace BlockEditor.Views.Windows
{
    public partial class BlockOptionWindow : Window
    {

        private readonly CultureInfo _culture = CultureInfo.InvariantCulture;
        private readonly SimpleBlock _block;
        private readonly Map _map;
        private readonly string _mapItemOptions;

        public BlockOptionWindow(Map map, MyPoint? index)
        {
            InitializeComponent();

            if (map == null || index == null)
                return;


            _map = map;
            _block = GetBlock(index.Value);
            _mapItemOptions = ItemBlockOptionsControl.GetOptions(map.Level.Items.Select(i => i.ID));

            OpenWindows.Add(this);
            MyUtils.SetPopUpWindowPosition(this);
            Init(index.Value);
        }

        private SimpleBlock GetBlock(MyPoint index)
        {
            var block = _map.Blocks.StartBlocks.GetBlock(index);

            if (block.IsEmpty())
                block = _map.Blocks.GetBlock(index);

            return block;
        }

        private void Init(MyPoint index)
        {
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

                if(_block.IsItem())
                {
                    var c = new ItemBlockOptionsControl();

                    if(string.IsNullOrWhiteSpace(_block.Options))
                        c.SetItems(_map.Level.Items);
                    else
                        c.SetBlockOptions(_block.Options);

                    c.Margin = new Thickness(5,0,10,20);
                    c.OnBlockOptionChanged += OnOptionsChanged;
                    OptionPanel.Children.Add(c);
                }
                else if(_block.ID == Block.TELEPORT)
                {
                    var panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;
                    panel.VerticalAlignment = VerticalAlignment.Center;
                    panel.Margin= new Thickness(10, 20,10,10);
                  
                    var label = new TextBlock();
                    label.Text = "Color: ";
                    label.VerticalAlignment = VerticalAlignment.Center;
                    label.FontSize = 14;
                    label.Margin = new Thickness(0,0,0,20);
                    
                    var c = new ColorPickerControl();
                    c.VerticalAlignment = VerticalAlignment.Center;
                    c.SetColor(_block.Options);
                    c.Margin = new Thickness(5, 0, 10, 20);
                    c.OnNewColor += OnNewColor;
                    c.Height = 30;
                    
                    panel.Children.Add(label);
                    panel.Children.Add(c);
                    OptionPanel.Children.Add(panel);
                }
                else
                {
                    var c = new BlockOptionsControl();
                    c.SetBlockOptions(_block.Options);
                    c.Margin = new Thickness(10, 0, 10, 20);
                    c.OnBlockOptionChanged += OnOptionsChanged;
                    OptionPanel.Children.Add(c);
                }
            }
        }


        private void Window_Deactivated(object sender, EventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
                Close();
        }

        private void OnOptionsChanged(string text)
        {
            if (text == null || _block.IsEmpty())
                return;

            var ignoreOption = _block.IsItem() && string.Equals(text, _mapItemOptions, StringComparison.CurrentCultureIgnoreCase);

            var b = new SimpleBlock(_block.ID, _block.Position.Value, ignoreOption ? string.Empty : text);
            _map.Blocks.Add(b);

        }

        private void OnNewColor(string text)
        {
            try
            {
                var value = Convert.ToInt32(text, 16);
                text = value.ToString();
            }
            catch
            {
                MessageUtil.ShowError("Failed to convert color to PR2 block option format.");
            }

            OnOptionsChanged(text);
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenWindows.Remove(this);
        }
    }
}
