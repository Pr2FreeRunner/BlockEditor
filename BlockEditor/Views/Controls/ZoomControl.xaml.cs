using BlockEditor.ViewModels;
using System.Windows.Controls;

namespace BlockEditor.Views.Controls
{

    public partial class ZoomControl : UserControl
    {
        public ZoomViewModel ViewModel { get; }
        public ZoomControl()
        {
            InitializeComponent();
            this.DataContext = ViewModel = new ZoomViewModel();
        }
    }
}
