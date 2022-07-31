using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BlockEditor.Helpers;
using BlockEditor.Models;
using System.Drawing;

using Point = System.Windows.Point;
using static BlockEditor.Models.BlockImages;

namespace BlockEditor.ViewModels
{
    public class MapViewModel : NotificationObject
    {

        private GameImage _gameImage;

        private Point? _mousePosition;

        private Camera _camera;

        public BitmapImage MapContent
        {
            get => _gameImage?.GetImage(); 
        }

        public Func<int?> GetSelectedBlockID { get; set; }

        public GameEngine Engine { get; }

        public Map Map { get; private set; }


        public MapViewModel()
        {
            Map = new Map();
            Engine = new GameEngine();
            Engine.OnFrame += OnFrame;
            _camera = new Camera();
        }


        private Point? GetPosition(IInputElement src, MouseEventArgs e)
        {
            if (src == null || e == null)
                return null;

            var point = e.GetPosition(src);

            var x = point.X;
            var y = point.Y;

            return new Point(x, y);
        }

        private void AddBlock(Point? p)
        {
            var id = GetSelectedBlockID?.Invoke();

            if (p == null || id == null || Map == null)
                return;

            var x = p.Value.X + _camera.Position.X;
            var y = p.Value.Y + _camera.Position.Y;

            var pos   = new Point(x, y);
            var index = Map.GetMapIndex(pos);

            Map.Blocks.Add(index, id.Value);
        }

        private void DeleteBlock(Point? p)
        {
            if (p == null || Map == null)
                return;

            var x = p.Value.X + _camera.Position.X;
            var y = p.Value.Y + _camera.Position.Y;
            var pos = new Point(x, y);

            Map.Blocks.Delete(Map.GetMapIndex(pos));
        }

        public void GoToStartPosition()
        {
            var p = Map.Blocks.GetStartPosition();

            if (p == null)
                return;

            var size = Map.BlockSize.GetPixelSize();
            var x = p.Value.X * size - (_gameImage.Width / 2);
            var y = p.Value.Y * size - (_gameImage.Height / 2);

            _camera.Position = new MyPoint(x, y); ;
        }

        private void OnFrame()
        {
            new FrameUpdater(_gameImage, Map, _camera, _mousePosition, GetSelectedBlockID());

            RaisePropertyChanged(nameof(MapContent));
        }

        #region Events

        public void Map_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var p = GetPosition(sender as IInputElement, e);

            if (e.ChangedButton == MouseButton.Right)
            {
                DeleteBlock(p);
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                AddBlock(p);
            }
        }

        public void Map_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            _mousePosition = GetPosition(sender as IInputElement, e);

            if (e.RightButton == MouseButtonState.Pressed)
            {
                DeleteBlock(_mousePosition);
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (GetSelectedBlockID?.Invoke() == null)
                    return;

                AddBlock(_mousePosition);
            }
        }
    
        public void Map_SizeChanged(int width, int height)
        {
            // thread safe?
            _gameImage = new GameImage(width, height);
        }

        public void OnLoaded()
        {
            Engine.Start();
        }
      
        public void OnLoadMap(Map map)
        {
            if (map == null)
                return;

            Engine.Pause = true;
            Thread.Sleep(GameEngine.FPS * 5); // make sure engine has been stopped

            var size = Map.BlockSize;
            Map = map;
            Map.BlockSize = size;

            GoToStartPosition();

            Engine.Pause = false;
        }

        public void OnZoomChanged(BlockSize size)
        {
            var halfScreenX = _gameImage.Width  / 2;
            var halfScreenY = _gameImage.Height / 2;

            var cameraPosition = new Point(_camera.Position.X, _camera.Position.Y);
            var middleOfScreen = new Point(cameraPosition.X + halfScreenX, cameraPosition.Y + halfScreenY);

            var currentIndex = Map.GetMapIndex(middleOfScreen);
            var currentSize  = Map.BlockSize;

            Map.BlockSize = size;

            var x = currentIndex.X * Map.BlockPixelSize - halfScreenX;
            var y = currentIndex.Y * Map.BlockPixelSize - halfScreenY;

            _camera.Position = new MyPoint(x, y);
        }

        #endregion

    }
}
