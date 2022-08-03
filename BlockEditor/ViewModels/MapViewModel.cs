using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Utils;
using static BlockEditor.Models.BlockImages;

namespace BlockEditor.ViewModels
{
    public class MapViewModel : NotificationObject
    {
        public Game Game { get; }

        private MyPoint? _mousePosition;

        public UserMode Mode { get; set; }

        public BitmapImage MapContent
        {
            get => Game.GameImage?.GetImage(); 
        }

        public BlockSelection BlockSelection { get; }

        private Action _cleanBlockSelection { get; }


        public MapViewModel(Action cleanBlockSelection)
        {
            _cleanBlockSelection = cleanBlockSelection;
            Game = new Game();
            Game.Engine.OnFrame += OnFrameUpdate;
            Mode = UserMode.None;
            BlockSelection = new BlockSelection();
            BlockSelection.OnSelectionClick += OnSelectionClick;
        }


        #region Events
       
        private void OnSelectionClick()
        {
            Mode = UserMode.Selection;
            _cleanBlockSelection?.Invoke();
        }

        public void OnCleanUserMode()
        {
            _cleanBlockSelection?.Invoke();
            BlockSelection?.Clean();
            Mode = UserMode.None;
        }

        public void OnFrameUpdate()
        {
            new FrameUpdate(Game, _mousePosition, BlockSelection);

            RaisePropertyChanged(nameof(MapContent));
        }

        internal void OnSelectedBlockID(int? id)
        {
            BlockSelection.Clean(); 
            BlockSelection.SelectedBlock = id;

            if(id != null)
                Mode = UserMode.AddBlock;
        }

        public void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (Mode)
            {
                case UserMode.AddBlock:
                    var p1 = MyUtils.GetPosition(sender as IInputElement, e);

                    if (e.ChangedButton == MouseButton.Right)
                        Game.DeleteBlock(p1);
                    else if (e.ChangedButton == MouseButton.Left)
                        Game.AddBlock(p1, BlockSelection.SelectedBlock);
                    break;

                case UserMode.Selection:
                    var p2 = MyUtils.GetPosition(sender as IInputElement, e);

                    if (p2 == null)
                        break;

                    if (e.LeftButton == MouseButtonState.Pressed) 
                    { 
                        BlockSelection.UserSelection.OnMouseDown(p2, Game.GetMapIndex(p2));
                    }
                    else if (e.RightButton == MouseButtonState.Pressed)
                    { 
                        var click = Game.GetMapIndex(_mousePosition);
                        var start = BlockSelection.UserSelection.StartMapIndex;
                        var end   = BlockSelection.UserSelection.EndMapIndex;

                        if (click == null || start == null || end == null)
                            break;

                        if (click.Value.X < start.Value.X || click.Value.X >= end.Value.X)
                            break;

                        if (click.Value.Y < start.Value.Y || click.Value.Y >= end.Value.Y)
                            break;

                        Game.DeleteSelection(start, end);
                        OnCleanUserMode();
                    }


                    break;

                case UserMode.AddSelection:
                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    var p3 = MyUtils.GetPosition(sender as IInputElement, e);
                    Game.AddSelection(p3, BlockSelection.SelectedBlocks);
                    break;

                case UserMode.None:
                    var p4 = MyUtils.GetPosition(sender as IInputElement, e);

                    if (e.ChangedButton == MouseButton.Right)
                        Game.DeleteBlock(p4);

                    break;
            }
            
        }

        public void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            switch (Mode)
            {
                case UserMode.AddBlock:
                    _mousePosition = MyUtils.GetPosition(sender as IInputElement, e);

                    if (e.RightButton == MouseButtonState.Pressed)
                        Game.DeleteBlock(_mousePosition);
                    else if (e.LeftButton == MouseButtonState.Pressed)
                        Game.AddBlock(_mousePosition, BlockSelection.SelectedBlock);
                    break;

                case UserMode.Selection:
                    _mousePosition = MyUtils.GetPosition(sender as IInputElement, e);
                    break;

                case UserMode.AddSelection:
                    _mousePosition = MyUtils.GetPosition(sender as IInputElement, e);

                    if (e.LeftButton != MouseButtonState.Pressed)
                        break;

                    var p3 = MyUtils.GetPosition(sender as IInputElement, e);
                    Game.AddSelection(p3, BlockSelection.SelectedBlocks);
                    break;
            }
        }

        internal void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (Mode)
            {
                case UserMode.Selection:

                    if (e.ChangedButton != MouseButton.Left)
                        break;

                    var pos   = MyUtils.GetPosition(sender as IInputElement, e);
                    var index = Game.GetMapIndex(pos);

                    BlockSelection.UserSelection.OnMouseUp(pos, index);
                    break;
            }
        }

        public void OnSizeChanged(int width, int height)
        {
            if(Game.GameImage != null)
                Game.GameImage.Dispose();

           Game.Camera.ScreenSize = new MyPoint(width, height);
            Game.GameImage = new GameImage(width, height);  // thread safe?
        }
      
        public void OnLoadMap(Map map)
        {
            if (map == null)
                return;

            Game.Engine.Pause = true;
            Thread.Sleep(GameEngine.FPS * 5); // make sure engine has been stopped

            var size = Game.Map.BlockSize;
            Game.Map = map;
            Game.Map.BlockSize = size;

            Game.UserOperations.Clear();

            Game.GoToStartPosition();

            Game.Engine.Pause = false;
        }

        public void OnZoomChanged(BlockSize size)
        {
            var halfScreenX = Game.GameImage.Width  / 2;
            var halfScreenY = Game.GameImage.Height / 2;

            var cameraPosition = new MyPoint(Game.Camera.Position.X, Game.Camera.Position.Y);
            var middleOfScreen = new MyPoint(cameraPosition.X + halfScreenX, cameraPosition.Y + halfScreenY);

            var currentIndex = Game.Map.GetMapIndex(middleOfScreen);
            var currentSize  = Game.Map.BlockSize;

            Game.Map.BlockSize = size;

            var x = currentIndex.X * Game.Map.BlockPixelSize - halfScreenX;
            var y = currentIndex.Y * Game.Map.BlockPixelSize - halfScreenY;

            Game.Camera.Position = new MyPoint(x, y);
        }

        #endregion

    }
}
