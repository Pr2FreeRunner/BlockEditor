using BlockEditor.Helpers;
using BlockEditor.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BlockEditor.Views
{
    public partial class MapControl : UserControl
    {
        public readonly MapViewModel ViewModel;

        public MapControl()
        {
            InitializeComponent();
            this.DataContext = ViewModel = new MapViewModel();
            ViewModel.SelectedBlock = () => BlocksControl.SelectedBlock;

            MapButtons.ViewModel.OnLoadMap += ViewModel.LoadMap;
            MapButtons.ViewModel.OnSaveMap += () => MapUtil.Save(ViewModel.Map);
            MapButtons.ViewModel.OnTestMap += () => MapUtil.TestInTasTool(ViewModel.Map);

            ZoomControl.ViewModel.OnZoomChanged += ViewModel.OnZoomChanged;

            this.Loaded += windowLoaded;
        }

        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.GoToStartPosition();
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.OnLoaded(sender, e);
        }

        private void Map_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            ViewModel.Map_PreviewMouseDown(sender, e);
        }

        private void Map_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ViewModel.Map_PreviewMouseMove(sender, e);
        }

        private void Map_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            ViewModel.Map_SizeChanged(sender, e);
        }
    }
}
