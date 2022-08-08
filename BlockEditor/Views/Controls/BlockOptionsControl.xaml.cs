using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp3
{
    public partial class BlockOptionsControl : UserControl
    {

        public event Action<string> OnBlockOptionChanged;

        public BlockOptionsControl()
        {
            InitializeComponent();
        }

        public void SetBlockOptions(string input)
        {
            if (input == null)
                return;

            textbox.Text = input;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnBlockOptionChanged?.Invoke(textbox.Text);
        }
    }
}
