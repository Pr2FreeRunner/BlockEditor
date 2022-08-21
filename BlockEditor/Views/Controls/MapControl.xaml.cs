using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using BlockEditor.Helpers;
using BlockEditor.ViewModels;
using BlockEditor.Models;
using static BlockEditor.Models.UserMode;
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

            MapButtons.ViewModel.OnLoadMap += ViewModel.OnLoadMap;
            MapButtons.ViewModel.OnSaveMap += () => MapUtil.Save(ViewModel.Game.Map);
            MapButtons.ViewModel.OnTestMap += () => MapUtil.TestInTasTool(ViewModel.Game.Map);
            ZoomControl.ViewModel.OnZoomChanged += (zoom) => ViewModel.OnZoomChanged(zoom);
            this.Loaded += windowLoaded;
            ZoomControl.ViewModel.Zoom = MySettings.Zoom;
        }

        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.Game.GoToStartPosition();
                Keyboard.Focus(this);
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.Game.Engine.Start();
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        private void Map_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ViewModel.OnPreviewMouseDown(sender, e);

                if (MySettings.FirstTimeLoad)
                {
                    MessageUtil.ShowInfo("Hint:  You can delete a block by right-clicking it.");
                    MySettings.FirstTimeLoad = false;
                }
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        private void Map_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                ViewModel.OnPreviewMouseMove(sender, e);
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        private void Map_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ViewModel.OnPreviewMouseUp(sender, e);
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }

        }

        private void Map_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                ViewModel.OnSizeChanged((int)GamePanel.ActualWidth, (int)GamePanel.ActualHeight);
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        public void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (ZoomControl.ViewModel.ZoomInCommand.CanExecute(null))
                    ZoomControl.ViewModel.ZoomInCommand.Execute(null);
            }
            else if (e.Delta < 0)
            {
                if (ZoomControl.ViewModel.ZoomOutCommand.CanExecute(null))
                    ZoomControl.ViewModel.ZoomOutCommand.Execute(null);
            }
        }

        public void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

                if (e.Key == Key.Escape)
                {
                    ViewModel.OnCleanUserMode(true);
                    return;
                }
                else if (ctrl && e.Key == Key.Z)
                {
                    ViewModel.Game.UserOperations.Undo();
                }
                else if (ctrl && e.Key == Key.Y)
                {
                    ViewModel.Game.UserOperations.Redo();
                }
                else if (ctrl && e.Key == Key.OemPlus)
                {
                    if(ZoomControl.ViewModel.ZoomInCommand.CanExecute(null))
                        ZoomControl.ViewModel.ZoomInCommand.Execute(null);
                }
                else if (ctrl && e.Key == Key.OemMinus)
                {
                    if (ZoomControl.ViewModel.ZoomOutCommand.CanExecute(null))
                        ZoomControl.ViewModel.ZoomOutCommand.Execute(null);
                }
                else if (ctrl && e.Key == Key.A)
                {
                    if (MapButtons.ViewModel.AccountCommand.CanExecute(null))
                        MapButtons.ViewModel.AccountCommand.Execute(null);
                }
                else if (ctrl && e.Key == Key.S)
                {
                    if (MapButtons.ViewModel.SaveCommand.CanExecute(null))
                        MapButtons.ViewModel.SaveCommand.Execute(null);
                }
                else if (ctrl && e.Key == Key.L)
                {
                    if (MapButtons.ViewModel.LoadCommand.CanExecute(null))
                        MapButtons.ViewModel.LoadCommand.Execute(null);
                }
                else if (ctrl && e.Key == Key.T)
                {
                    if (MapButtons.ViewModel.TestCommand.CanExecute(null))
                        MapButtons.ViewModel.TestCommand.Execute(null);
                }
                else if (ctrl && e.Key == Key.N)
                {
                    if (MapButtons.ViewModel.NewCommand.CanExecute(null))
                        MapButtons.ViewModel.NewCommand.Execute(null);
                }
                else if (IsSelectionKey(e, ctrl))
                {
                    ViewModel.UserSelection.CreateSelection(ViewModel.Game.Map);

                    if (e.Key == Key.X || e.Key == Key.Delete)
                        ViewModel.Game.DeleteSelection(ViewModel.UserSelection.MapRegion);

                    ViewModel.OnCleanUserMode(e.Key == Key.Delete);
                }
                else if(ctrl && e.Key == Key.V)
                {
                    BlockSelection.ActivatePreviousSelection();;
                }
                else if (BlockSelection.SelectedBlocks != null && e.Key == Key.R)
                {
                    BlockSelection.Rotate();
                }
                else if (e.Key == Key.S)
                {
                    if (ViewModel.SelectCommand.CanExecute(null))
                        ViewModel.SelectCommand.Execute(null);
                }
                else if (e.Key == Key.B)
                {
                    if (ViewModel.BlockInfoCommand.CanExecute(null))
                        ViewModel.BlockInfoCommand.Execute(null);
                }
                else if (e.Key == Key.G)
                {
                    if (ViewModel.StartPositionCommand.CanExecute(null))
                        ViewModel.StartPositionCommand.Execute(null);
                }
                else if (e.Key == Key.M)
                {
                    if (ViewModel.MapInfoCommand.CanExecute(null))
                        ViewModel.MapInfoCommand.Execute(null);
                }
                else if (e.Key == Key.I)
                {
                    if (ViewModel.AddImageCommand.CanExecute(null))
                        ViewModel.AddImageCommand.Execute(null);
                }
                else if (e.Key == Key.A)
                {
                    if (ViewModel.AddShapeCommand.CanExecute(null))
                        ViewModel.AddShapeCommand.Execute(null);
                }
                else if (e.Key == Key.C)
                {
                    if (ViewModel.BlockCountCommand.CanExecute(null))
                        ViewModel.BlockCountCommand.Execute(null);
                }
                else if (e.Key == Key.V)
                {
                    if (ViewModel.VerticalFlipCommand.CanExecute(null))
                        ViewModel.VerticalFlipCommand.Execute(null);
                }
                else if (e.Key == Key.O)
                {
                    ViewModel.IsOverwrite = !ViewModel.IsOverwrite;
                }
                else if (e.Key == Key.F)
                {
                    ViewModel.OnFillClick();
                }
                else if (e.Key == Key.R)
                {
                    if (ViewModel.ReplaceCommand.CanExecute(null))
                        ViewModel.ReplaceCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        private bool IsSelectionKey(KeyEventArgs e, bool ctrl)
        {
            var isSelectionMode = ViewModel.Mode.Value == UserModes.Selection;
            var isCopy = ctrl && (e.Key == Key.C || e.Key == Key.X);
            var isDelete = e.Key == Key.Delete;

            return isSelectionMode && (isCopy || isDelete);
        }

    }
}
