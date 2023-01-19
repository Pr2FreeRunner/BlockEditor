using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlockEditor.Models
{
    public class ToolWindow : Window
    {

        public ToolWindow()
        {
            this.MouseLeftButtonDown += ToolWindow_MouseLeftButtonDown;
            this.Closing += ToolWindow_Closing;

            OpenWindows.Add(this);
        }

        private void ToolWindow_Closing(object sender, CancelEventArgs e)
        {
            OpenWindows.Remove(this);
        }

        public void ToolWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ButtonState == MouseButtonState.Released)
                return;

            try 
            { 
                base.OnMouseLeftButtonDown(e);
                this.DragMove();
            }
            catch { } 
        }

        public void Integer_PreviewTextInput(object sender, TextCompositionEventArgs e, int? min, int? max)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            var culture = CultureInfo.InvariantCulture;
            bool isInteger = int.TryParse(fullText, NumberStyles.Integer, culture, out var result);
            var minValid = min != null ? result >= min : true;
            var maxValid = max != null ? result <= max : true;
            var allowNegative = min == null || (min != null && min < 0);

            if (allowNegative && string.Equals(fullText, "-", StringComparison.InvariantCultureIgnoreCase))
                return;

            e.Handled = !isInteger || !minValid || !maxValid;
        }

        public void Double_PreviewTextInput(object sender, TextCompositionEventArgs e, int? min = null, int? max = null)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            var culture = CultureInfo.InvariantCulture;
            bool isDouble = double.TryParse(fullText, NumberStyles.Any, culture, out var result);
            var minValid = min != null ? result >= min : true;
            var maxValid = max != null ? result <= max : true;
            var allowNegative = min == null || (min != null && min < 0);

            if (allowNegative && string.Equals(fullText, "-", StringComparison.InvariantCultureIgnoreCase))
                return;

            e.Handled = !isDouble || !minValid || !maxValid;
        }
    }
}
