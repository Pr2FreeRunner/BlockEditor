using System.Windows;
using BlockEditor.Models;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

using static BlockEditor.Models.BlockImages;
using System;

namespace BlockEditor.Views.Controls
{
    public partial class BlocksControl : UserControl
    {

        public event Action<int?> OnSelectedBlockID;

        private Border _selectedBorder { get; set; }


        public BlocksControl()
        {
            InitializeComponent();

            AddBlocks();
        }


        private void AddBlocks()
        {
            foreach (var image in BlockImages.GetAllImageBlocks(BlockSize.Zoom100))
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
            image.Tag = block.ID;
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
            var id     = (border?.Child as Image).Tag as int?;

            if (id == null)
                return;

            CreateSelection(border, id);
        }

        private void CreateSelection(Border border, int? id)
        {
            ToggleBorder(_selectedBorder);

            _selectedBorder = border;
            OnSelectedBlockID?.Invoke(id);

            ToggleBorder(_selectedBorder);
        }

        public void RemoveSelection()
        {
            CreateSelection(null, null);
        }
    }
}
