using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using BlockEditor.Helpers;
using BlockEditor.ViewModels;
using BlockEditor.Models;
using BlockEditor.Views.Windows;

using static BlockEditor.Models.UserMode;
using static BlockEditor.Models.BlockImages;

namespace BlockEditor.Views.Controls
{

    public partial class MapControl : UserControl
    {
        public readonly MapViewModel ViewModel;
        private bool _firstload = false;
        public MapControl()
        {
            InitializeComponent();
            this.DataContext = ViewModel = new MapViewModel();

            MapButtons.ViewModel.OnLoadMap += ViewModel.OnLoadMap;
            MapButtons.ViewModel.OnSaveMap += () => SaveMapUtil.Save(ViewModel.Game.Map);
            MapButtons.ViewModel.OnTestMap += () => MapUtil.TestInTasTool(ViewModel.Game.Map);
            ZoomControl.ViewModel.OnZoomChanged += (zoom) => ViewModel.OnZoomChanged(zoom);
            this.Loaded += windowLoaded;
            ZoomControl.ViewModel.Zoom = MySettings.Zoom;
        }

        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if(!_firstload)
                {
                    ViewModel.Game.GoToStartPosition();
                    _firstload = true;
                }

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
                ViewModel.IsOverwrite = MySettings.Overwrite; //updates GUI
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
                    var hint1 = "Hint 1:  You can delete a block by right-clicking it.";
                    var hint2 = "Hint 2:  You can deselect anything with the Escape key.";
                    MessageUtil.ShowInfo(hint1 + Environment.NewLine + Environment.NewLine + hint2);
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
            var ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            if(!ctrl)
                return;

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
                    if (ViewModel.Commands.DeselectCommand.CanExecute(null))
                        ViewModel.Commands.DeselectCommand.Execute(null);
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
                    if (ZoomControl.ViewModel.ZoomInCommand.CanExecute(null))
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
                else if (IsSelectionKey(e, ctrl))
                {
                    ViewModel.Game.UserSelection.CreateSelection(ViewModel.Game.Map);

                    if (e.Key == Key.X || e.Key == Key.Delete)
                        ViewModel.Game.DeleteSelection(ViewModel.Game.UserSelection.MapRegion);

                    var del = e.Key == Key.Delete;
                    ViewModel.Game.CleanUserMode(del, del);
                }
                else if (ctrl && e.Key == Key.V)
                {
                    BlockSelection.ActivatePreviousSelection();
                }        
                else if (ctrl && e.Key == Key.I)
                {
                    if (ViewModel.Commands.InfoCommand.CanExecute(null))
                        ViewModel.Commands.InfoCommand.Execute(null);
                }
                else if (ctrl && e.Key == Key.T)
                {
                    if (ViewModel.Commands.TransformCommand.CanExecute(null))
                        ViewModel.Commands.TransformCommand.Execute(null);
                }
                else if (ctrl && e.Key == Key.D)
                {
                    if (ViewModel.Commands.DeleteCommand.CanExecute(null))
                        ViewModel.Commands.DeleteCommand.Execute(null);
                }
                else if (ctrl && e.Key == Key.E)
                {
                    if (ViewModel.Commands.EditCommand.CanExecute(null))
                        ViewModel.Commands.EditCommand.Execute(null);
                }
                else if (ctrl && e.Key == Key.N)
                {
                    if (ViewModel.Commands.NavigatorCommand.CanExecute(null))
                        ViewModel.Commands.NavigatorCommand.Execute(null);
                }
                else if (ctrl && e.Key == Key.Q)
                {
                    if (ViewModel.Commands.AdvancedCommand.CanExecute(null))
                        ViewModel.Commands.AdvancedCommand.Execute(null);
                }
                else if (ctrl && e.Key == Key.B)
                {
                    if (ViewModel.Commands.BuildCommand.CanExecute(null))
                        ViewModel.Commands.BuildCommand.Execute(null);
                }
                else if (ctrl && e.Key == Key.O)
                {
                    ViewModel.IsOverwrite = !ViewModel.IsOverwrite;
                }
                else if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                {
                    if (ViewModel.Commands.SelectCommand.CanExecute(null))
                        ViewModel.Commands.SelectCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        private bool IsSelectionKey(KeyEventArgs e, bool ctrl)
        {
            var isSelectionMode = ViewModel.Game.Mode.Value == UserModes.Selection;
            var isCopy = ctrl && (e.Key == Key.C || e.Key == Key.X);
            var isDelete = e.Key == Key.Delete;

            return isSelectionMode && (isCopy || isDelete);
        }

    }
}
