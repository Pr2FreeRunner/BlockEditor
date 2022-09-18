using BlockEditor.Models;
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
    public partial class BuildImageWindow : ToolWindow
    {
        private static double? _posX;
        private static double? _posY;

        private static string _path;
        private static int? _target;
        private static int? _size;
        private static int? _ignoreColor;
        private static int? _sensitivity;
        private static string _posXInput;
        private static string _posYInput;

        public BuildDTO BuildInfo { get; set; }

        public bool GetPosition { get; set; }


        public BuildImageWindow(MyPoint? p)
        {
            InitializeComponent();
            Init(p);
            UpdateButtons();
        }


        private void UpdateButtons()
        {
            btnOk.IsEnabled = IsInputValid();

            var target = GetTarget();
            sensitivityPanel.Visibility = target == ImageType.Blocks ? Visibility.Collapsed : Visibility.Visible;
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

        private ImageType? GetTarget()
        {
            if(cbTarget.SelectedIndex == -1)
                return null;

            return (ImageType)(cbTarget.SelectedIndex + 1);
        }

        private void AddSizes(ImageType? target)
        {
            var index = cbSize.SelectedIndex;
            cbSize.Items.Clear();
            
            var count = (target == null || target.Value != ImageType.Blocks) ? 8 : 20;

            for (int i = 1; i <= count; i++)
            {
                var item = new ComboBoxItem();
                item.Content = i.ToString();

                cbSize.Items.Add(item);
            }

            if(index >= 0 && index < count)
                cbSize.SelectedIndex = index;
        }

        private void Init(MyPoint? p)
        {
            foreach (ImageType type in Enum.GetValues(typeof(ImageType)))
            {
                var item = new ComboBoxItem();
                var name = MyUtil.InsertSpaceBeforeCapitalLetter(type.ToString());
                item.Content = name;

                if(!string.Equals("None", name, StringComparison.InvariantCultureIgnoreCase))
                    cbTarget.Items.Add(item);
            }

            foreach (IgnoreColor type in Enum.GetValues(typeof(IgnoreColor)))
            {
                var item = new ComboBoxItem();
                item.Content = MyUtil.InsertSpaceBeforeCapitalLetter(type.ToString()); ;

                cbIgnoreColor.Items.Add(item);
            }

            foreach (ColorSensitivty type in Enum.GetValues(typeof(ColorSensitivty)))
            {
                var item = new ComboBoxItem();
                item.Content = MyUtil.InsertSpaceBeforeCapitalLetter(type.ToString()); ;

                cbSensitivity.Items.Add(item);
            }

            AddSizes(_target == null ? null : (ImageType?) _target.Value);

            if(p != null)
                tbPosX.Text = p.Value.X.ToString() + ".0";
            else if(_posXInput != null)
                tbPosX.Text = _posXInput;

            if (p != null)
                tbPosY.Text = p.Value.Y.ToString() + ".0";
            else if (_posY != null)
                tbPosY.Text = _posYInput;

            if (_target != null)
                cbTarget.SelectedIndex = _target.Value;

            if (_sensitivity != null)
                cbSensitivity.SelectedIndex = _sensitivity.Value;
            else
                cbSensitivity.SelectedIndex = (int)(ColorSensitivty.High - 1);

            if (_ignoreColor != null)
                cbIgnoreColor.SelectedIndex = _ignoreColor.Value;

            if (_size != null)
                cbSize.SelectedIndex = _size.Value;

            if (_path != null)
                tbPath.Text = _path; 
        }

        private void Double_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Double_PreviewTextInput(sender, e, 0, 2000);
        }

        private void SaveInputs()
        {
            _path = tbPath.Text;
            _size = cbSize.SelectedIndex;
            _target = cbTarget.SelectedIndex;
            _ignoreColor = cbIgnoreColor.SelectedIndex;
            _sensitivity = cbSensitivity.SelectedIndex;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveInputs();
        }

        private void tbPosX_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (MyUtil.TryParseDouble(tb.Text, out var result))
            {
                _posX = result;
                _posXInput = tb.Text;
            }

            UpdateButtons();
        }

        private void tbPosY_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (MyUtil.TryParseDouble(tb.Text, out var result))
            {
                _posY = result;
                _posYInput = tb.Text;
            }

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

            if(BuildInfo.ImageInfo.Image == null)
                throw new Exception("Failed to load the image.");
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var target = GetTarget();
                int? id = Block.BASIC_WHITE;

                if(target == null)
                    throw new Exception("Invalid input, no target selected.");

                if(target.Value == ImageType.Blocks)
                {
                    id = SelectBlockWindow.Show("Block to Add:", false);

                    if (id == null)
                        return;
                }

                SetBuildInfo(target.Value, id.Value);

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
                throw new Exception("Filepath to image is invalid.");

            try
            {
                using (var stream = new SKManagedStream(File.OpenRead(path)))
                    return SKBitmap.Decode(stream);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Image file not found.");
                return null;
            }
            catch(Exception ex)
            {
                throw new Exception("Failed to load the image." + Environment.NewLine + Environment.NewLine + ex.Message);
            }
        }

        public void ShiftPosition(List<Block> blocks)
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

        private void btnPosition_Click(object sender, RoutedEventArgs e)
        {
            GetPosition = true;
            DialogResult = true;
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
                Close();
        }

        private void target_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddSizes(GetTarget());
            UpdateButtons();
        }

    }
}
