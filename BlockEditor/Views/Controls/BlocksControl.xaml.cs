using BlockEditor.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static BlockEditor.Models.BlockImages;

namespace BlockEditor.Views.Controls
{
    public partial class BlocksControl : UserControl
    {

        public BlockImage SelectedBlock { get; private set; }

        private Border _selectedBorder { get; set; }


        public BlocksControl()
        {
            InitializeComponent();

            AddBlocks();
        }


        private void AddBlocks()
        {
            foreach (var image in BlockImages.GetImageBlocks(BlockSize.Normal))
            {
                if (image == null)
                    continue;

                BlockContainer.Children.Add(CreateBorder(image));
            }
        }

        private Border CreateBorder(BlockImage block)
        {
            var border = new Border();
            border.Margin = new Thickness(3, 6, 3, 6);
            border.BorderThickness = new Thickness(3);
            border.MouseDown += Border_MouseDown;

            var image  = new Image();
            image.Source = block.Image;
            image.Tag = block;
            border.Child = image;

            return border;
        }

        private void ToggleBorder(Border border)
        {
            if (border == null)
                return;

            if (border.BorderBrush == null)
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            else
                _selectedBorder.BorderBrush = null;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            var border = sender as Border;
            var block  = (border?.Child as Image).Tag as BlockImage;

            if (block == null)
                return;

            ToggleBorder(_selectedBorder);

            _selectedBorder = border;
            SelectedBlock   = block;

            ToggleBorder(_selectedBorder);
        }

    }
}
