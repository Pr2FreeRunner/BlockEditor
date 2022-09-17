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
using System.Windows.Media;

namespace BlockEditor.Views.Windows
{
    public partial class BlockOptionWindow : ToolWindow
    {

        private readonly CultureInfo _culture = CultureInfo.InvariantCulture;
        private SimpleBlock _block;
        private readonly Map _map;
        private readonly string _mapItemOptions;
        private Action _refreshGui;

        public bool StartNavigation { get; set; }

        public BlockOptionWindow(Map map, MyPoint? index, Action refreshGui)
        {
            InitializeComponent();

            if (map == null || index == null)
                return;

            _refreshGui = refreshGui;
            _map = map;
            _block = GetBlock(index.Value);
            _mapItemOptions = ItemBlockOptionsControl.GetOptions(map.Level.Items.Select(i => i.ID));

            Init(index.Value);
            MyUtil.SetPopUpWindowPosition(this);
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
                lblPosX.Text = index.X.ToString(_culture);
                lblPosY.Text = index.Y.ToString(_culture);
                tbBlockOption.Text = String.Empty;
            }
            else
            {
                BlockImage.Source = BlockImages.GetImageBlock(BlockImages.BlockSize.Zoom160, _block.ID)?.Image;
                lblPosX.Text = _block.Position.Value.X.ToString(_culture);
                lblPosY.Text = _block.Position.Value.Y.ToString(_culture);

                if (_block.IsItem())
                {
                    var c = new ItemBlockOptionsControl();

                    if (string.IsNullOrWhiteSpace(_block.Options))
                        c.SetItems(_map.Level.Items);
                    else
                        c.SetBlockOptions(_block.Options);

                    c.Margin = new Thickness(5, 0, 10, 20);
                    c.OnBlockOptionChanged += OnOptionsChanged;
                    OptionPanel.Children.Add(c);
                }
                else if (_block.ID == Block.TELEPORT)
                {
                    var panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;
                    panel.VerticalAlignment = VerticalAlignment.Center;
                    panel.Margin = new Thickness(10, 10, 10, 5);

                    var label = new TextBlock();
                    label.Text = "Color: ";
                    label.VerticalAlignment = VerticalAlignment.Center;
                    label.FontSize = 14;
                    label.Margin = new Thickness(0, 0, 0, 15);
                    label.FontWeight = FontWeights.SemiBold;

                    var c = new ColorPickerControl();
                    c.VerticalAlignment = VerticalAlignment.Center;
                    c.SetColor(_block.Options);
                    c.Margin = new Thickness(5, 0, 5, 15);
                    c.OnNewColor += OnNewColor;
                    c.Height = 30;

                    var b = new WhiteButton("Navigate to connected blocks");
                    b.HorizontalAlignment = HorizontalAlignment.Left;
                    b.VerticalAlignment = VerticalAlignment.Center;
                    b.OnClick += btnNavigate_Click;
                    b.Width = 210;
                    b.Height = 26;
                    b.Margin = new Thickness(10, 0, 0, 10);

                    panel.Children.Add(label);
                    panel.Children.Add(c);
                    OptionPanel.Children.Add(panel);
                    OptionPanel.Children.Add(b);
                }
                else if(_block.ID == Block.HAPPY_BLOCK || _block.ID == Block.SAD_BLOCK)                
                {
                    tbBlockOption.Text = "Stats Change:";
                    var c = new StatsBlockControl(_block.ID == Block.SAD_BLOCK);
                    c.SetBlockOptions(_block.Options);
                    c.Margin = new Thickness(10, 0, 10, 20);
                    c.OnBlockOptionChanged += OnOptionsChanged;
                    OptionPanel.Children.Add(c);
                }
                else if (_block.ID == Block.CUSTOM_STATS)
                {
                    tbBlockOption.Text = string.Empty;
                    var c = new CustomStatsControl();
                    c.SetBlockOptions(_block.Options);
                    c.Margin = new Thickness(10, 0, 10, 20);
                    c.OnBlockOptionChanged += OnOptionsChanged;
                    OptionPanel.Children.Add(c);
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

        private void btnConnected_Click()
        {
            var count = 0;

            using (new TempCursor(Cursors.Wait))
            {
                foreach (var b in _map.Blocks.GetBlocks())
                {
                    if (b.ID != Block.TELEPORT)
                        continue;

                    if (!string.Equals(b.Options, _block.Options, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    count++;
                }
            }

            if (count <= 1)
                MessageUtil.ShowInfo($"There is no connected teleport blocks.");
            else if (count == 2)
                MessageUtil.ShowInfo($"There is 1 connected teleport block.");
            else
                MessageUtil.ShowInfo($"There are {count - 1} connected teleport blocks.");
        }

        private void btnNavigate_Click()
        {
            StartNavigation = true;
            Close();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void OnOptionsChanged(string text)
        {
            if (text == null || _block.IsEmpty())
                return;

            var ignoreOption = _block.IsItem() && string.Equals(text, _mapItemOptions, StringComparison.CurrentCultureIgnoreCase);

            _block = new SimpleBlock(_block.ID, _block.Position.Value, ignoreOption ? string.Empty : text);

            using(new TempOverwrite(_map.Blocks, true))
                _map.Blocks.Add(_block);
        }

        private void OnNewColor(string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                    text = Convert.ToInt32(text, 16).ToString();
            }
            catch
            {
                MessageUtil.ShowError("Failed to convert color to PR2 block option format.");
            }

            OnOptionsChanged(text);
            _refreshGui?.Invoke();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
