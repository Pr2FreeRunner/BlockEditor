using BlockEditor.Models;
using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BlockEditor.Views
{
    public partial class BlocksControl : UserControl
    {

        public ImageBlock SelectedBlock { get; private set; }

        private Border _selectedBorder { get; set; }

        public BlocksControl()
        {
            InitializeComponent();

            AddBlocks(GetFiles());
        }

        private string[] GetFiles()
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Blocks");
                return Directory.GetFiles(path);
            }
            catch
            {
                return null;
            }
        }

        private ImageBlock GetImage(string filepath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filepath))
                    return null;

                var filename = Path.GetFileNameWithoutExtension(filepath);

                if(!int.TryParse(filename, NumberStyles.Any , CultureInfo.InvariantCulture, out var id))
                    return null;

                var image = new BitmapImage(new Uri(filepath));

                if(image == null)
                    return null;

                return new ImageBlock { ID=id, Source=image };
            }
            catch
            {
                Debugger.Break();
                return null;
            }
        }

        private void AddBlocks(string[] files)
        {
            if (files == null)
                return;


            foreach (var file in files)
            {
                var block = GetImage(file);

                if (block == null)
                    continue;

                BlockContainer.Children.Add(CreateBorder(block));
            }
        }

        private Border CreateBorder(ImageBlock block)
        {
            var border = new Border();
            border.Margin = new Thickness(3, 6, 3, 6);
            border.MouseDown += Border_MouseDown;
            border.BorderThickness = new Thickness(3);
            border.Child = block;

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

            var border  = sender as Border;
            var block = border?.Child as ImageBlock;

            if (block == null)
                return;

            ToggleBorder(_selectedBorder);

            _selectedBorder = border;
            SelectedBlock   = block;

            ToggleBorder(_selectedBorder);
        }

    }
}
