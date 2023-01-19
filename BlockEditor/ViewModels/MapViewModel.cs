using System;
using System.Windows;
using System.Windows.Input;
using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Utils;
using BlockEditor.Views.Controls;
using SkiaSharp;

using static BlockEditor.Models.BlockImages;
using static BlockEditor.Models.UserMode;

namespace BlockEditor.ViewModels
{

    public class MapViewModel : NotificationObject
    {

        public bool IsOverwrite
        {
            get { return Game.Map?.Blocks.Overwrite ?? false; }
            set { Game.Map.Blocks.Overwrite = value; RaisePropertyChanged(); }
        }

        public bool ShowArt
        {
            get { return Game.ShowArt; }
            set 
            { 
                Game.ShowArt = value; 
                RaisePropertyChanged(); 
            }
        }

        public bool ShowArtIsEnabled
        {
            get { return Game.ShowArtIsEnabled(); }
        }

        public Game Game { get; }
        public MenuCommands Commands { get; }
        public bool DisableAddBlockOnMouseMove { get; set; }

        public MapViewModel()
        {
            Game = new Game();
            Commands = new MenuCommands(Game);
        }



        public void OnFrameUpdate(SKSurface surface)
        {
            new FrameUpdate(Game, surface);
        }

        public void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DisableAddBlockOnMouseMove = false;
            var p = MyUtil.GetPosition(sender as IInputElement, e);
            var index = Game.GetMapIndex(p);

            if (p == null || index == null)
                return;

            switch (Game.Mode.Value)
            {
                case UserModes.Delete:
                    if (Game.UserSelection.MapRegion.IsInside(index))
                        Game.DeleteBlocks(Game.UserSelection.MapRegion);
                    else
                        Game.DeleteBlock(index);
                    break;

                case UserModes.Distance:
                    Game.MeasureDistance.Reset();
                    Game.MeasureDistance.MapPoint1 = index;
                    Game.MeasureDistance.ImagePoint1 = p;
                    break;

                case UserModes.GetPosition:
                    if (Game.GetPosition != null)
                        Game.GetPosition(index);
                    else
                        Game.Mode.Value = UserModes.None;
                    break;

                case UserModes.ConnectTeleports:
                    var c = App.MyMainWindow?.CurrentMap?.GetSidePanel() as ConnectTeleportsControl;

                    if(c != null)
                        c.Add(Game.Map.Blocks.GetBlock(index));

                    break;

                case UserModes.Selection:

                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Game.UserSelection.OnMouseDown(p);
                    }
                    else if (e.RightButton == MouseButtonState.Pressed)
                    {
                        if (!Game.UserSelection.MapRegion.IsInside(index))
                            break;

                        Game.DeleteBlocks(Game.UserSelection.MapRegion);
                        Game.CleanUserMode(true, true);
                    }

                    break;

                case UserModes.BlockInfo:
                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    if (p == null)
                        break;

                    Commands.Tools.StartBlockInfo(Game, index);
                    break;

                case UserModes.Fill:
                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    if (Game.UserSelection.HasSelectedRegion && !Game.UserSelection.MapRegion.IsInside(index))
                    {
                        MessageUtil.ShowInfo("You clicked outside the selected region."
                            + Environment.NewLine
                            + Environment.NewLine
                            + "Either remove the selected region or click inside it.");

                        return;
                    }

                    Commands.Tools.StartFloodFill(Game, index);
                    break;

                default:
                    if (e.ChangedButton == MouseButton.Right)
                    {
                        Game.DeleteBlock(index);
                    }
                    else if (e.ChangedButton == MouseButton.Left)
                    {
                        var ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
                        if (ctrl && Commands.Tools.BlockInfoCommand.CanExecute(null))
                        {
                            Commands.Tools.BlockInfoCommand.Execute(null);
                            OnPreviewMouseDown(sender, e);
                            return;
                        }

                        if (BlockSelection.SelectedBlocks != null)
                            Game.AddBlocks(BlocksUtil.MoveSelection(BlockSelection.SelectedBlocks, index));
                        else if (BlockSelection.SelectedBlock != null)
                            Game.AddBlock(index, BlockSelection.SelectedBlock);
                        else
                        {
                            var b = Game.Map.Blocks.GetBlock(index, true);

                            if (!b.IsEmpty())
                            {
                                Game.DeleteBlock(b);
                                BlockSelection.SelectedBlock = b.ID;
                                BlockSelection.SelectedBlockOption = b.Options;
                                Game.Mode.Value = UserModes.MoveBlock;
                            }
                        }
                    }
                    break;
            }
        }

        public void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            Game.MousePosition = MyUtil.GetPosition(sender as IInputElement, e);
            var index = Game.GetMapIndex(Game.MousePosition);

            switch (Game.Mode.Value)
            {
                case UserModes.Delete:
                    if (e.RightButton == MouseButtonState.Pressed || e.LeftButton == MouseButtonState.Pressed)
                    {
                        if (Game.UserSelection.MapRegion.IsInside(index))
                            Game.DeleteBlocks(Game.UserSelection.MapRegion);
                        else
                            Game.DeleteBlock(index);
                    }
                    break;

                case UserModes.MoveBlock:
                    break;

                default:
                    if (e.RightButton == MouseButtonState.Pressed)
                    {
                        Game.DeleteBlock(index);
                    }
                    else if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        if(DisableAddBlockOnMouseMove)
                            break;

                        if (BlockSelection.SelectedBlocks != null)
                            Game.AddBlocks(BlocksUtil.MoveSelection(BlockSelection.SelectedBlocks, index));
                        else
                            Game.AddBlock(index, BlockSelection.SelectedBlock);
                    }
                    break;
            }
        }

        public void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var p = MyUtil.GetPosition(sender as IInputElement, e);
            var index = Game.GetMapIndex(p);

            if (p == null || index == null)
                return;

            switch (Game.Mode.Value)
            {
                case UserModes.Distance:
                    Game.MeasureDistance.MapPoint2 = index;
                    Game.MeasureDistance.ImagePoint2 = p;

                    MessageUtil.ShowInfo(Game.MeasureDistance.GetDistance());
                    break;

                case UserModes.MoveBlock:
                    if (BlockSelection.SelectedBlock != null)
                        Game.AddBlock(index, BlockSelection.SelectedBlock.Value, BlockSelection.SelectedBlockOption);

                    BlockSelection.Reset();
                    Game.Mode.Value = UserModes.None;
                    break;


                case UserModes.Selection:

                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    Game.UserSelection.OnMouseUp(p);

                    if (MySettings.FirstUserSelection
                        && Game.UserSelection.HasSelectedRegion
                        && Game.UserSelection.SelectedRegionContainsBlocks(Game.Map))
                    {
                        MessageUtil.ShowInfo("Hint:  To copy the blocks inside the selected region press Ctrl + C");
                        MySettings.FirstUserSelection = false;
                    }
                    break;
            }
        }

        public void OnSizeChanged(int width, int height)
        {
            Game.Camera.ScreenSize = new MyPoint(width, height);
        }

        public void OnLoadMap(Map map)
        {
            if (map == null)
                return;

            Game.Engine.PauseConfirmed();

            var size = Game.Map.BlockSize;
            Game.Map.Dispose();
            Game.Map = map;
            Game.Map.BlockSize = size;

            Game.UserOperations.Clear();
            App.MyMainWindow?.TitleChanged(Game.Map.Level.Title);
            Game.CleanUserMode(true, true);
            Game.GoToStartPosition();

            MyUtil.BlocksOutsideBoundries(map.BlocksOutsideBoundries);

            Game.Engine.Pause = false;
        }

        public void OnZoomChanged(BlockSize size)
        {
            var halfScreenX = Game.Camera.ScreenSize.X / 2;
            var halfScreenY = Game.Camera.ScreenSize.Y / 2;

            var cameraPosition = new MyPoint(Game.Camera.Position.X, Game.Camera.Position.Y);
            var middleOfScreen = new MyPoint(cameraPosition.X + halfScreenX, cameraPosition.Y + halfScreenY);

            var currentIndex = Game.Map.GetMapIndex(middleOfScreen);
            var currentSize = Game.Map.BlockSize;

            Game.Map.BlockSize = size;

            var x = currentIndex.X * Game.Map.BlockPixelSize - halfScreenX;
            var y = currentIndex.Y * Game.Map.BlockPixelSize - halfScreenY;

            Game.Camera.Position = new MyPoint(x, y);
            RaisePropertyChanged(nameof(ShowArtIsEnabled));
            RaisePropertyChanged(nameof(ShowArt));
        }

    }
}
