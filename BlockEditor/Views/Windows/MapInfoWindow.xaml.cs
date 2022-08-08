using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp3
{
    public partial class MapInfoWindow : Window
    {
        public MapInfoWindow()
        {
            InitializeComponent();
            ItemBlockOptionsControl.OnBlockOptionChanged += OnBlockOptionChanged;
        }

        private void OnBlockOptionChanged(string obj)
        {
        }

        private void Integer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox   = sender as TextBox;
            var fullText  = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            var culture   = CultureInfo.InvariantCulture;
            bool isInteger = !int.TryParse(fullText, NumberStyles.Integer, culture, out var result);

            e.Handled = isInteger && result >= 0;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Time_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CowboyHat_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Title_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Background_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
