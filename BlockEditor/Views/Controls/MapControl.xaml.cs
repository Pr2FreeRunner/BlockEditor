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
            ViewModel.GetSelectedBlockID = () => BlocksControl.SelectedBlockId;

            MapButtons.ViewModel.OnLoadMap += ViewModel.OnLoadMap;
            MapButtons.ViewModel.OnSaveMap += () => MapUtil.Save(ViewModel.Map);
            MapButtons.ViewModel.OnTestMap += () => MapUtil.TestInTasTool(ViewModel.Map);

            ZoomControl.ViewModel.OnZoomChanged += (zoom) => ViewModel.OnZoomChanged(zoom);

            this.Loaded += windowLoaded;

            ZoomControl.ViewModel.Init();
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

            ViewModel.OnPreviewMouseDown(sender, e);
        }

        private void Map_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ViewModel.OnPreviewMouseMove(sender, e);
        }

        private void Map_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel.OnSizeChanged((int)GamePanel.ActualWidth, (int)GamePanel.ActualHeight);
        }

        private void OnZoomChanged(BlockSize size)
        {
            if(ViewModel?.Map == null)
                return;

            ViewModel.Map.BlockSize = size;
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                BlocksControl.SelectedBlockId = null;
                return;
            }

            var ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            if (e.Key == Key.Z && ctrl)
            {
                ViewModel.UserOperations.Undo();
                return;
            }

            if (e.Key == Key.Y && ctrl)
            {
                ViewModel.UserOperations.Redo();
                return;
            }
        }
    }
}
