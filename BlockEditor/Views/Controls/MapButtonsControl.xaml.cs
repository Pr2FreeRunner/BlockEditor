using BlockEditor.ViewModels;
using System;
using System.Windows.Controls;

namespace BlockEditor.Views.Controls
{
    public partial class MapButtonsControl : UserControl
    {
        public MapButtonsViewModel ViewModel { get; }

        public MapButtonsControl()
        {
            InitializeComponent();

            this.DataContext = ViewModel= new MapButtonsViewModel();
        }
    }
}
