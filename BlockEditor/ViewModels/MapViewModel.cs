using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Utils;
using static BlockEditor.Models.BlockImages;
using static BlockEditor.Models.UserModes;

namespace BlockEditor.ViewModels
{
    public class MapViewModel : NotificationObject
    {
        public Game Game { get; }

        private Point? _mousePosition;

        public UserMode Mode { get; set; }

        public BitmapImage MapContent
        {
            get => Game.GameImage?.GetImage(); 
        }

        public int? SelectedBlockID { get; set; }

        public MapViewModel()
        {
            Game = new Game();
            Game.Engine.OnFrame += OnFrameUpdate;
            Mode = UserMode.None;
        }

        #region Events

        public void OnFrameUpdate()
        {
            new FrameUpdate(Game, _mousePosition, SelectedBlockID);

            RaisePropertyChanged(nameof(MapContent));
        }

        internal void OnSelectedBlockID(int? id)
        {
            SelectedBlockID = id;
            Mode = id != null ? UserMode.AddBlock : UserMode.None;
        }

        public void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (Mode)
            {
                case UserMode.AddBlock:
                    var p = MyUtils.GetPosition(sender as IInputElement, e);

                    if (e.ChangedButton == MouseButton.Right)
                        Game.DeleteBlock(p);
                    else if (e.ChangedButton == MouseButton.Left)
                        Game.AddBlock(p, SelectedBlockID);
                    break;

                case UserMode.Selection:
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
                        Game.AddBlock(_mousePosition, SelectedBlockID);
                    break;
            }
            
        }

        internal void OnPreviewMouseUp(object sender, MouseEventArgs e)
        {
            switch (Mode)
            {
                case UserMode.Selection:
                    _mousePosition = MyUtils.GetPosition(sender as IInputElement, e);
                    break;
            }
        }

        public void OnSizeChanged(int width, int height)
        {
            if(Game.GameImage != null)
                Game.GameImage.Dispose();

           
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

            var cameraPosition = new Point(Game.Camera.Position.X, Game.Camera.Position.Y);
            var middleOfScreen = new Point(cameraPosition.X + halfScreenX, cameraPosition.Y + halfScreenY);

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
