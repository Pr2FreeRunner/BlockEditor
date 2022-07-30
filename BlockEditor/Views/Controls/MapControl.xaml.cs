using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using BlockEditor.Helpers;
using BlockEditor.ViewModels;
using static BlockEditor.Models.BlockImages;

namespace BlockEditor.Views.Controls
{

    public partial class MapControl : UserControl
    {
        public readonly MapViewModel ViewModel;

        public MapControl()
        {
            InitializeComponent();
            this.DataContext = ViewModel = new MapViewModel();
            ViewModel.GetSelectedBlock = () => BlocksControl.SelectedBlock;

            MapButtons.ViewModel.OnLoadMap += ViewModel.OnLoadMap;
            MapButtons.ViewModel.OnSaveMap += () => MapUtil.Save(ViewModel.Map);
            //MapButtons.ViewModel.OnTestMap += () => MapUtil.TestInTasTool(ViewModel.Map);
            MapButtons.ViewModel.OnTestMap += () => ViewModel.GoToStartPosition();

            ZoomControl.ViewModel.OnZoomChanged += (zoom) => ViewModel.OnZoomChanged(zoom);

            this.Loaded += windowLoaded;
        }

        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.GoToStartPosition();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.OnLoaded();
        }

        private void Map_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            ViewModel.Map_PreviewMouseDown(sender, e);
        }

        private void Map_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ViewModel.Map_PreviewMouseMove(sender, e);
        }

        private void Map_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel.Map_SizeChanged((int)GamePanel.ActualWidth, (int)GamePanel.ActualHeight);
        }

        private void OnZoomChanged(BlockSize size)
        {
            if(ViewModel?.Map == null)
                return;

            ViewModel.Map.BlockSize = size;
        }
    }
}
