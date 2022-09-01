﻿using BlockEditor.Models;
using BlockEditor.Utils;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Input;
using Builders.DataStructures.DTO;
using Microsoft.Win32;
using SkiaSharp;
using System.IO;
using BlockEditor.Helpers;
using System.Collections.Generic;
using LevelModel.Models.Components;
using LevelModel.Models.Components.Art;

using static Builders.DataStructures.DTO.ImageDTO;
using static Builders.DataStructures.DTO.BuildDTO;

namespace BlockEditor.Views.Windows
{
    public partial class ImageToBlocksWindow : Window
    {
        private static double? _posX;
        private static double? _posY;

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

            var target = (ImageType) (cbTarget.SelectedIndex + 1);

            sensitivityPanel.Visibility =  target == ImageType.Blocks ? Visibility.Collapsed : Visibility.Visible;
        }

        private bool IsInputValid()
        {
            if (_posX == null)
                return false;

            if (_posY == null)
                return false;

            if (string.IsNullOrWhiteSpace((cbSize.SelectedValue as ComboBoxItem)?.Content as string))
                return false;

            if (string.IsNullOrWhiteSpace((cbIgnoreColor.SelectedValue as ComboBoxItem)?.Content as string))
                return false;

            if (string.IsNullOrWhiteSpace((cbSensitivity.SelectedValue as ComboBoxItem)?.Content as string))
                return false;

            if (string.IsNullOrWhiteSpace((cbTarget.SelectedValue as ComboBoxItem)?.Content as string))
                return false;

            if (string.IsNullOrWhiteSpace(tbPath.Text))
                return false;

            if (!File.Exists(tbPath.Text))
                return false;

            return true;
        }

        private void Init()
        {
            tbPosX.Text = "444.0";
            tbPosY.Text = "337.0";

            foreach (ImageType type in Enum.GetValues(typeof(ImageType)))
            {
                var item = new ComboBoxItem();
                var name = MyUtils.InsertSpaceBeforeCapitalLetter(type.ToString());
                item.Content = name;

                if(!string.Equals("None", name, StringComparison.InvariantCultureIgnoreCase))
                    cbTarget.Items.Add(item);
            }

            foreach (IgnoreColor type in Enum.GetValues(typeof(IgnoreColor)))
            {
                var item = new ComboBoxItem();
                item.Content = MyUtils.InsertSpaceBeforeCapitalLetter(type.ToString()); ;

                cbIgnoreColor.Items.Add(item);
            }

            foreach (ColorSensitivty type in Enum.GetValues(typeof(ColorSensitivty)))
            {
                var item = new ComboBoxItem();
                item.Content = MyUtils.InsertSpaceBeforeCapitalLetter(type.ToString()); ;

                cbSensitivity.Items.Add(item);
            }

            for (int i = 1; i <= 8; i++)
            {
                var item = new ComboBoxItem();
                item.Content = i.ToString();

                cbSize.Items.Add(item);
            }

            cbSensitivity.SelectedIndex = (int)(ColorSensitivty.High - 1);
        }

        private void Double_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            var culture = CultureInfo.InvariantCulture;
            bool isDouble = double.TryParse(fullText, NumberStyles.Any, culture, out var result);

            e.Handled = !isDouble || result < 0;
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

            if (MyUtils.TryParseDouble(tb.Text, out var result))
                _posX = result;

            UpdateButtons();
        }

        private void tbPosY_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (MyUtils.TryParseDouble(tb.Text, out var result))
                _posY = result;

            UpdateButtons();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SetBuildInfo(ImageType target, int id)
        {
            BuildInfo = new BuildDTO()
            {
                Type = BuildType.Image,
                Title = string.Empty,
            };

            BuildInfo.ImageInfo.Type            = target;
            BuildInfo.ImageInfo.BlockType       = id;
            BuildInfo.ImageInfo.Size            = cbSize.SelectedIndex + 1;
            BuildInfo.ImageInfo.ColorToIgnore   = (IgnoreColor)(cbIgnoreColor.SelectedIndex + 1);
            BuildInfo.ImageInfo.Sensitivty      = (ColorSensitivty)(cbSensitivity.SelectedIndex + 1);
            BuildInfo.ImageInfo.Image           = LoadImage(tbPath.Text);
            BuildInfo.ImageInfo.Position        = ImagePosition.Custom;
            BuildInfo.ImageInfo.CreateDrawImage = true;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var target = (ImageType)(cbTarget.SelectedIndex + 1);
                int? id = Block.BASIC_WHITE;

                if(target == ImageType.Blocks)
                {
                    id = SelectBlockWindow.Show("Block to Add:", false);

                    if (id == null)
                        return;
                }

                SetBuildInfo(target, id.Value);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        private void Path_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                tbPath.Text = openFileDialog.FileName;
            }
            else
            {
                tbPath.Text = string.Empty;
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
            catch(Exception ex)
            {
                throw new Exception("Failed to load Image..." + Environment.NewLine + Environment.NewLine + ex.Message);
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
            start.X = ((int)_posX.Value) - edgeX;
            start.Y = ((int)_posY.Value) - edgeY;
        }

        public void ShiftPosition(IList<DrawArt> art)
        {
            if (art == null || !art.Any())
                return;

            if(_posX == null || _posY == null)
                return;

            var paddingX = (int) (30 * (_posX.Value - ((int)_posX)));
            var paddingY = (int) (30 * (_posY.Value - ((int)_posY)));
            var posX = (int) _posX * 30;
            var posY = (int)_posY * 30;


            for (int i = 0; i < art.Count; i++)
            {
                art[i].X += posX + paddingX;
                art[i].Y += posY + paddingY;
            }
        }

        private void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtons();
        }
    }
}