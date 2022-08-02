using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using BlockEditor.Helpers;
using BlockEditor.ViewModels;

namespace BlockEditor.Views.Controls
{

    public partial class MapControl : UserControl
    {
        public readonly MapViewModel ViewModel;

        public MapControl()
        {
            InitializeComponent();
            this.DataContext = ViewModel = new MapViewModel();

            MapButtons.ViewModel.OnLoadMap += ViewModel.OnLoadMap;
            MapButtons.ViewModel.OnSaveMap += () => MapUtil.Save(ViewModel.Game.Map);
            MapButtons.ViewModel.OnTestMap += () => MapUtil.TestInTasTool(ViewModel.Game.Map);
            ZoomControl.ViewModel.OnZoomChanged += (zoom) => ViewModel.OnZoomChanged(zoom);
            BlocksControl.OnSelectedBlockID += ViewModel.OnSelectedBlockID;
            this.Loaded += windowLoaded;

            ZoomControl.ViewModel.Init();
        }

        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Game.GoToStartPosition();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Game.Engine.Start();
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

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                BlocksControl.RemoveSelection();
                return;
            }

            var ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            if (e.Key == Key.Z && ctrl)
            {
                ViewModel.Game.UserOperations.Undo();
                return;
            }

            if (e.Key == Key.Y && ctrl)
            {
                ViewModel.Game.UserOperations.Redo();
                return;
            }
        }

        private void Map_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.OnPreviewMouseUp(sender, e);

        }
    }
}
