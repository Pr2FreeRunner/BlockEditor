using BlockEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlockEditor.Views
{
    /// <summary>
    /// Interaction logic for MapButtons.xaml
    /// </summary>
    public partial class MapButtons : UserControl
    {
        public MapButtonsViewModel ViewModel { get; }

        public MapButtons()
        {
            InitializeComponent();

            this.DataContext = ViewModel= new MapButtonsViewModel();
        }
    }
}
