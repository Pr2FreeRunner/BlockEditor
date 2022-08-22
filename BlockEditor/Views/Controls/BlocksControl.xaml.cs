using System.Windows;
using BlockEditor.Models;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

using static BlockEditor.Models.BlockImages;
using System;
using System.Linq;
using BlockEditor.Utils;
using LevelModel.Models.Components;
using System.Collections.Generic;

namespace BlockEditor.Views.Controls
{
    public partial class BlocksControl : UserControl
    {

        public event Action<int?> OnSelectedBlockID;

        private Border _selectedBorder { get; set; }

        private Color _selectedColor;
        private int _paddingX;

        public BlocksControl()
        {
            InitializeComponent();
            _selectedColor = Colors.White;
            _paddingX = 3;
        }

        public void Init(BlockSize size, int columnCount, bool whiteSelection = true)
        {
            _paddingX = size > BlockSize.Zoom100 ? 3 : 1;
            _selectedColor = whiteSelection ? Colors.White : Colors.Black;
            var images = GetAllImageBlocks(size).ToList();
            var rowCount = images.Count / columnCount + 1;

            BlockContainer.Children.Clear();
            BlockContainer.RowDefinitions.Clear();
            BlockContainer.ColumnDefinitions.Clear();

            for (int i = 0; i < columnCount; i++)
                BlockContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            for (int i = 0; i < rowCount; i++)
                BlockContainer.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            var count = 0;
            foreach (var image in images)
            {
                if (image == null)
                    continue;

                var border = CreateBorder(image);
                Grid.SetColumn(border, count % columnCount);
                Grid.SetRow(border, count / columnCount);

                BlockContainer.Children.Add(border);
                count++;
            }
        }

        private Border CreateBorder(BlockImage block)
        {
            var border = new Border();
            border.Margin = new Thickness(_paddingX, 6, _paddingX, 6);
            border.BorderThickness = new Thickness(3);
            border.MouseDown += Border_MouseDown;
            border.HorizontalAlignment = HorizontalAlignment.Stretch;
            border.VerticalAlignment = VerticalAlignment.Stretch;

            var image  = new Image();

            image.Source = block.Image;
            image.Tag = block.ID;
            border.Child = image;
            image.HorizontalAlignment = HorizontalAlignment.Stretch;
            image.VerticalAlignment = VerticalAlignment.Stretch;

            return border;
        }

        private void ToggleBorder(Border border)
        {
            if (border == null)
                return;

            if (border.BorderBrush == null)
                border.BorderBrush = new SolidColorBrush(_selectedColor);
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

            if(id != null)
                OnSelectedBlockID?.Invoke(id);

            ToggleBorder(_selectedBorder);
        }

        public void RemoveSelection()
        {
            CreateSelection(null, null);
        }

        public void OnKeyDown(Key k)
        {
            var id = MySettings.GetBlockId(k);

            if(id == null)
                return;

            foreach (var child in BlockContainer.Children)
            {
                var border = child as Border;
                var blockId = (border?.Child as Image).Tag as int?;

                if(blockId == null)
                    continue;

                if(blockId.Value == id)
                {
                    CreateSelection(border, id);
                    return;
                }
            }

        }
    }
}
