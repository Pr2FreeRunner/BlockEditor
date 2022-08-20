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

namespace BlockEditor.Views.Windows
{
    public partial class ImageToBlocksWindow : Window
    {
        private static int? _posX;
        private static int? _posY;
        private static int? _size;
        private static string _ignoreColor;
        private static string _imagePath;

        public List<SimpleBlock> Result { get; set; }

        public ImageToBlocksWindow()
        {
            InitializeComponent();
            Result = new List<SimpleBlock>();

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

            tbPosX.Text = _posX?.ToString(culture) ?? "100";
            tbPosX.Text = _posY?.ToString(culture) ?? "100";
            cbIgnoreColor.Text = _ignoreColor?.ToString(culture) ?? "";
            cbSize.Text = _size?.ToString(culture) ?? "";
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
            var cb = sender as ComboBox;

            if (cb == null)
                return;

            if (MyUtils.TryParse(cb.Text, out var result) && result > 0 && result <= 8)
                _size = result;
            else
                _size = null;

            UpdateButtons();
        }

        private void IgnoreColor_TextChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;

            if (cb == null)
                return;

            if (string.Equals(cb.Text, "Black", StringComparison.InvariantCultureIgnoreCase))
                _ignoreColor = cb.Text;
            else if (string.Equals(cb.Text, "White", StringComparison.InvariantCultureIgnoreCase))
                _ignoreColor = cb.Text;
            else if (string.Equals(cb.Text, "None", StringComparison.InvariantCultureIgnoreCase))
                _ignoreColor = cb.Text;
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
                Build();
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        private void Build()
        {
            var info = new BuildDTO()
            {
                Type = BuildDTO.BuildType.Image,
                Title = string.Empty,
            };

            info.ImageInfo.Type = ImageDTO.ImageType.Blocks;
            info.ImageInfo.Size = _size.Value;
            info.ImageInfo.ColorToIgnore = GetIgnoreColor();
            info.ImageInfo.Image = LoadImage(_imagePath);

            var level = Builders.PR2Builder.BuildLevel(info);
            var blocks = MyConverters.ToBlocks(level.Blocks, out var blocksOutsideBoundries);

            for (int x = 0; x < Blocks.SIZE; x++)
            {
                for (int y = 0; y < Blocks.SIZE; y++)
                {
                    var b = blocks.GetBlock(x, y);

                    if(b.IsEmpty())
                        continue;

                    Result.Add(b);
                }
            }

            MyUtils.BlocksOutsideBoundries(blocksOutsideBoundries);
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


        protected SKBitmap LoadImage(string path)
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
    }
}
