using BlockEditor.ViewModels;
using System.Windows.Controls;

namespace BlockEditor.Views
{
    public partial class MapControl : UserControl
    {
        private readonly MapViewModel _vm;

        public MapControl()
        {
            InitializeComponent();
            this.DataContext = _vm = new MapViewModel();
            _vm.SelectedBlock = () => BlocksControl.SelectedBlock;

            MapButtons.ViewModel.OnLoadMap += _vm.LoadMap;

        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _vm.OnLoaded(sender, e);
        }

        private void Map_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            _vm.Map_PreviewMouseDown(sender, e);
        }

        private void Map_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _vm.Map_PreviewMouseMove(sender, e);
        }

        private void Map_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            _vm.Map_SizeChanged(sender, e);
        }
    }
}
