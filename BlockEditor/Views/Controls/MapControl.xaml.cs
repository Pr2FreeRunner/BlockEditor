using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using BlockEditor.Helpers;
using BlockEditor.ViewModels;
using BlockEditor.Models;
using BlockEditor.Utils;

namespace BlockEditor.Views.Controls
{

    public partial class MapControl : UserControl
    {
        public readonly MapViewModel ViewModel;
        private bool _firstload = false;


        public MapControl()
        {
            InitializeComponent();
            ClearSidePanel();
            this.DataContext = ViewModel = new MapViewModel();

            MapButtons.ViewModel.OnLoadMap += ViewModel.OnLoadMap;
            MapButtons.ViewModel.OnSaveMap += () => SaveMapUtil.Save(ViewModel.Game.Map);
            MapButtons.ViewModel.OnTestMap += () => TasUtil.Start(ViewModel.Game.Map);
            this.Loaded += windowLoaded;
            ZoomControl.ViewModel.Zoom = MySettings.Zoom;
        }



        public void ClearSidePanel()
        {
            if(SidePanel?.Children == null)
                return;

            SidePanel.Children.Clear();
            SidePanel.Visibility = Visibility.Collapsed;
        }

        public Control GetSidePanel()
        {
            if(SidePanel?.Children == null)
                return null;

            if(SidePanel.Children.Count == 0)
                return null;

            return SidePanel.Children[0] as Control;
        }

        public void AddSidePanel(Control c)
        {
            // Don't use side panel...
            // It is buggy (weird consequences)
            // only use if there is no other way..
            if (c == null)
                return;

            SidePanel.Children.Clear();
            SidePanel.Children.Add(c);
            SidePanel.Visibility = Visibility.Visible;
        }

        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if(!_firstload)
                {
                    ViewModel.Game.GoToStartPosition();
                    ViewModel.Game.Engine.OnFrame += SKControl.InvalidateVisual;
                    _firstload = true;
                }

                Keyboard.Focus(this);
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }
        private bool IsClickInsideSidePanel(MouseButtonEventArgs e)
        {
            var panel = GetSidePanel();

            if (panel == null)
                return false;

            var p = e.GetPosition(panel);

            if(p.X < 0)
                return false;

            if (p.Y < 0)
                return false;

            if (p.X > panel.ActualWidth)
                return false;

            if(p.Y > panel.ActualHeight)
                return false;

            return true;
        }

        private void Map_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //refresh GUI
                ViewModel.IsOverwrite = MySettings.Overwrite; 
                ViewModel.ShowArt = MySettings.ShowArt;
                ZoomControl.ViewModel.Zoom = ZoomControl.ViewModel.Zoom;

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
                if(IsClickInsideSidePanel(e))
                    return;

                ViewModel.OnPreviewMouseDown(sender, e);
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
                var p = MyUtil.DipToPixels(new Point(GamePanel.ActualWidth, GamePanel.ActualHeight));
                ViewModel.OnSizeChanged(p.X, p.Y);
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        public void Map_OnPreviewMouseWheel(MouseWheelEventArgs e)
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

        public void Deselect()
        {
            if (ViewModel.Commands.DeselectCommand.CanExecute(null))
                ViewModel.Commands.DeselectCommand.Execute(null);
        }

        public void Map_OnPreviewKeyDown(KeyEventArgs e, bool ctrl)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                   Deselect();
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
                else if (ctrl && e.Key == Key.N)
                {
                    if (MapButtons.ViewModel.NewCommand.CanExecute(null))
                        MapButtons.ViewModel.NewCommand.Execute(null);
                }
                else if (ctrl && e.Key == Key.Q)
                {
                    if (MapButtons.ViewModel.TestCommand.CanExecute(null))
                        MapButtons.ViewModel.TestCommand.Execute(null);
                }
                else if (IsSelectionKey(e, ctrl))
                {
                    ViewModel.Game.UserSelection.CreateSelection(ViewModel.Game.Map);

                    if (e.Key == Key.X || e.Key == Key.Delete)
                        ViewModel.Game.DeleteBlocks(ViewModel.Game.UserSelection.MapRegion);

                    ViewModel.Game.CleanUserMode(e.Key == Key.Delete, true);
                }
                else if (ctrl && e.Key == Key.V)
                {
                    BlockSelection.ActivatePreviousSelection();
                }        
                else if (!ctrl && e.Key == Key.I)
                {
                    if (ViewModel.Commands.InfoCommand.CanExecute(null) && !App.IsSidePanelActive())
                        ViewModel.Commands.InfoCommand.Execute(null);
                }
                else if (!ctrl && e.Key == Key.T)
                {
                    if (ViewModel.Commands.TransformCommand.CanExecute(null) && !App.IsSidePanelActive())
                        ViewModel.Commands.TransformCommand.Execute(null);
                }
                else if (!ctrl && e.Key == Key.R)
                {
                    if (ViewModel.Commands.DeleteCommand.CanExecute(null) && !App.IsSidePanelActive())
                        ViewModel.Commands.DeleteCommand.Execute(null);
                }
                else if (!ctrl && e.Key == Key.E)
                {
                    if (ViewModel.Commands.EditCommand.CanExecute(null) && !App.IsSidePanelActive())
                        ViewModel.Commands.EditCommand.Execute(null);
                }
                else if (!ctrl && e.Key == Key.N)
                {
                    if (ViewModel.Commands.NavigatorCommand.CanExecute(null) && !App.IsSidePanelActive())
                        ViewModel.Commands.NavigatorCommand.Execute(null);
                }
                else if (!ctrl && e.Key == Key.Q)
                {
                    if (ViewModel.Commands.AdvancedCommand.CanExecute(null) && !App.IsSidePanelActive())
                        ViewModel.Commands.AdvancedCommand.Execute(null);
                }
                else if (!ctrl && e.Key == Key.B)
                {
                    if (ViewModel.Commands.BuildCommand.CanExecute(null) && !App.IsSidePanelActive())
                        ViewModel.Commands.BuildCommand.Execute(null);
                }
                else if (!ctrl && e.Key == Key.O)
                {
                    if(!App.IsSidePanelActive())
                        ViewModel.IsOverwrite = !ViewModel.IsOverwrite;
                }
                else if (!ctrl && e.Key == Key.LeftShift || e.Key == Key.RightShift)
                {
                    if (ViewModel.Commands.SelectCommand.CanExecute(null) && !App.IsSidePanelActive())
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
            var isSelectionMode = ViewModel.Game.UserSelection.HasSelectedRegion;
            var isCopy = ctrl && (e.Key == Key.C || e.Key == Key.X);
            var isDelete = e.Key == Key.Delete;

            return isSelectionMode && (isCopy || isDelete);
        }

        private void SKControl_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            ViewModel.OnFrameUpdate(e.Surface);
        }
    }
}
