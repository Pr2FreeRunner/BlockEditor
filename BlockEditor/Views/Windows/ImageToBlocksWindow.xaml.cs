using BlockEditor.Models;
using BlockEditor.Utils;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Builders.DataStructures.DTO;
using Microsoft.Win32;
using SkiaSharp;
using System.IO;
using BlockEditor.Helpers;
using System.Collections.Generic;
using LevelModel.Models;
using LevelModel.Models.Components;
using System.Linq;

namespace BlockEditor.Views.Windows
{
    public partial class ImageToBlocksWindow : Window
    {
        private static int? _posX;
        private static int? _posY;
        private static int? _size;
        private static string _ignoreColor;
        private static string _imagePath;

        public BuildDTO BuildInfo { get; set; }

        public ImageToBlocksWindow()
        {
            InitializeComponent();

            OpenWindows.Add(this);
            MyUtils.SetPopUpWindowPosition(this);

            Init();
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            btnOk.IsEnabled = IsInputValid();
        }

        private bool IsInputValid()
        {
            if (_posX == null)
                return false;

            if (_posY == null)
                return false;

            if (_size == null)
                return false;

            if (string.IsNullOrWhiteSpace(_ignoreColor))
                return false;

            if (string.IsNullOrWhiteSpace(_imagePath))
                return false;

            if (!File.Exists(_imagePath))
                return false;

            return true;
        }

        private void Init()
        {
            var culture = CultureInfo.InvariantCulture;

            tbPosX.Text = _posX?.ToString(culture) ?? "350";
            tbPosY.Text = _posY?.ToString(culture) ?? "350";
            cbIgnoreColor.Text = _ignoreColor?.ToString(culture) ?? "";
            cbSize.Text = _size?.ToString(culture) ?? "";
            tbPath.Text = _imagePath ?? string.Empty;
        }

        private void Integer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            var culture = CultureInfo.InvariantCulture;
            bool isInteger = int.TryParse(fullText, NumberStyles.Integer, culture, out var result);

            e.Handled = !isInteger || result < 0;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenWindows.Remove(this);
        }

        private void tbPosX_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (MyUtils.TryParse(tb.Text, out var result))
                _posX = result;

            UpdateButtons();
        }

        private void tbPosY_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (MyUtils.TryParse(tb.Text, out var result))
                _posY = result;

            UpdateButtons();
        }

        private void Size_TextChanged(object sender, SelectionChangedEventArgs e)
        {
            var text = ((sender as ComboBox)?.SelectedItem as ComboBoxItem)?.Content as string;

            if (text == null)
                return;

            if (MyUtils.TryParse(text, out var result) && result >= 0 && result <= 8)
                _size = result;
            else
                _size = null;

            UpdateButtons();
        }

        private void IgnoreColor_TextChanged(object sender, SelectionChangedEventArgs e)
        {
            var text = ((sender as ComboBox)?.SelectedItem as ComboBoxItem)?.Content as string;

            if (text == null)
                return;

            if (string.Equals(text, "Black", StringComparison.InvariantCultureIgnoreCase))
                _ignoreColor = text;
            else if (string.Equals(text, "White", StringComparison.InvariantCultureIgnoreCase))
                _ignoreColor = text;
            else if (string.Equals(text, "None", StringComparison.InvariantCultureIgnoreCase))
                _ignoreColor = text;
            else
                _ignoreColor = string.Empty;

            UpdateButtons();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var id = SelectBlockWindow.Show("Block to Add:", false);

                if (id == null)
                    return;

                BuildInfo = new BuildDTO()
                {
                    Type = BuildDTO.BuildType.Image,
                    Title = string.Empty,
                };

                BuildInfo.ImageInfo.BlockType = id.Value;
                BuildInfo.ImageInfo.Type = ImageDTO.ImageType.Blocks;
                BuildInfo.ImageInfo.Size = _size.Value;
                BuildInfo.ImageInfo.ColorToIgnore = GetIgnoreColor();
                BuildInfo.ImageInfo.Image = LoadImage(_imagePath);
                BuildInfo.ImageInfo.Position = ImageDTO.ImagePosition.Custom;

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        private ImageDTO.IgnoreColor GetIgnoreColor()
        {
            if (string.Equals(_ignoreColor, "Black", StringComparison.InvariantCultureIgnoreCase))
                return ImageDTO.IgnoreColor.Black;
            else if (string.Equals(_ignoreColor, "White", StringComparison.InvariantCultureIgnoreCase))
                return ImageDTO.IgnoreColor.White;
            else if (string.Equals(_ignoreColor, "None", StringComparison.InvariantCultureIgnoreCase))
                return ImageDTO.IgnoreColor.None;

            throw new Exception("Invalid 'Color to Ignore' configuration");
        }

        private void Path_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                tbPath.Text = openFileDialog.FileName;
                _imagePath = openFileDialog.FileName;
            }
            else
            {
                tbPath.Text = string.Empty;
                _imagePath = string.Empty;
            }

            UpdateButtons();
        }

        private SKBitmap LoadImage(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            try
            {
                using (var stream = new SKManagedStream(File.OpenRead(path)))
                    return SKBitmap.Decode(stream);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Image file not found...");
                return null;
            }
        }

        public static void ShiftPosition(List<Block> blocks)
        {
            if(blocks == null || !blocks.Any())
                return;

            bool first = true;
            int edgeX  = 0;
            int edgeY  = 0;
            int posX   = 0;
            int posY   = 0;

            foreach (var b in blocks)
            {
                if(first)
                {
                    b.X = 0;
                    b.Y = 0;
                    first = false;
                    continue;
                }

                posX += b.X;
                posY += b.Y;

                if(posX < edgeX)
                    edgeX = posX;

                if (posY < edgeY)
                    edgeY = posY;
            }

            var start = blocks.First();
            start.X = _posX.Value - edgeX;
            start.Y = _posY.Value - edgeY;
        }
    }
}
