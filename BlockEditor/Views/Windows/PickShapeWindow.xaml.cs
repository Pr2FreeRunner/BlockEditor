﻿using BlockEditor.Models;
using BlockEditor.Utils;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static BlockEditor.Utils.ShapeBuilderUtil;

namespace BlockEditor.Views.Windows
{

    public partial class PickShapeWindow : Window
    {
        public ShapeType Result { get; private set; }
        public bool Fill => MySettings.FillShape;

        public int Probability { get; set; }

        public PickShapeWindow(ShapeType fallback)
        {
            InitializeComponent();
            Probability = 100;
            Result = fallback;
            FillCheckbox.IsChecked = Fill;
            OpenWindows.Add(this);
        }

        private void btnRectangle_Click(object sender, RoutedEventArgs e)
        {
            Result = ShapeType.Rectangle;
            DialogResult = true;
            Close();
        }

        private void btnSquare_Click(object sender, RoutedEventArgs e)
        {
            Result = ShapeType.Square;
            DialogResult = true;
            Close();
        }

        private void btnCircle_Click(object sender, RoutedEventArgs e)
        {
            Result = ShapeType.Circle;
            DialogResult = true;
            Close();
        }

        private void btnEllipse_Click(object sender, RoutedEventArgs e)
        {
            Result = ShapeType.Ellipse;
            DialogResult = true;
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }

        private void FillCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            MySettings.FillShape = true;
        }

        private void FillCheckbox_UnChecked(object sender, RoutedEventArgs e)
        {
            MySettings.FillShape = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenWindows.Remove(this);
        }

        private void tb_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (tb == null)
                return;

            if (MyUtil.TryParse(tb.Text, out var result))
                Probability = result;
            else
            {
                Probablity = 100;
            }
        }

        private void Integer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            var culture = CultureInfo.InvariantCulture;
            bool isDouble = int.TryParse(fullText, NumberStyles.Any, culture, out var result);

            e.Handled = !isDouble || result < 0 || result > 100;
        }

    }
}
