﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using BlockEditor.Helpers;
using BlockEditor.ViewModels;
using BlockEditor.Models;

namespace BlockEditor.Views.Controls
{

    public partial class MapControl : UserControl
    {
        public readonly MapViewModel ViewModel;

        public MapControl()
        {
            InitializeComponent();
            this.DataContext = ViewModel = new MapViewModel(CleanBlocksControlSelection);

            MapButtons.ViewModel.OnLoadMap += ViewModel.OnLoadMap;
            MapButtons.ViewModel.OnSaveMap += () => MapUtil.Save(ViewModel.Game.Map);
            MapButtons.ViewModel.OnTestMap += () => MapUtil.TestInTasTool(ViewModel.Game.Map);
            ZoomControl.ViewModel.OnZoomChanged += (zoom) => ViewModel.OnZoomChanged(zoom);
            BlocksControl.OnSelectedBlockID += ViewModel.OnSelectedBlockID;
            this.Loaded += windowLoaded;

            ZoomControl.ViewModel.Init();
            this.Focus();
        }

        private void CleanBlocksControlSelection()
        {
            BlocksControl.RemoveSelection();
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

        private void Map_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.OnPreviewMouseUp(sender, e);

        }
  
        private void Map_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel.OnSizeChanged((int)GamePanel.ActualWidth, (int)GamePanel.ActualHeight);
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            
            if(e.Key == Key.Escape)
            {
                ViewModel.OnCleanUserMode();
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
            else if(IsSelectionKey(e, ctrl))
            {
                var startPoint = ViewModel.BlockSelection.UserSelection.StartMapIndex;
                var endPoint   = ViewModel.BlockSelection.UserSelection.EndMapIndex;

                ViewModel.BlockSelection.UserSelection.OnKeydown(ViewModel.Game.Map);

                if(e.Key == Key.X || e.Key == Key.Delete)
                    ViewModel.Game.DeleteSelection(startPoint, endPoint);

                if(e.Key == Key.Delete)
                    ViewModel.OnCleanUserMode();
                else
                    ViewModel.Mode = UserMode.AddSelection;
            }
            else if(ViewModel.Mode == UserMode.AddSelection && e.Key == Key.L)
            {
                ViewModel.BlockSelection.RotateLeft();
            }
            else if (ViewModel.Mode == UserMode.AddSelection && e.Key == Key.R)
            {
                ViewModel.BlockSelection.RotateRight();
            }
            else if (e.Key == Key.S && ViewModel.Mode != UserMode.Selection)
            {
                ViewModel.BlockSelection.SelectionActivation();
            }
        }

        private bool IsSelectionKey(KeyEventArgs e, bool ctrl)
        {
            var isSelectionMode = ViewModel.Mode == UserMode.Selection;
            var isCopy = ctrl && (e.Key == Key.C || e.Key == Key.X);
            var isDelete = e.Key == Key.Delete;

            return isSelectionMode && (isCopy || isDelete);
        }

    }
}
