using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Models.UserOperations;
using BlockEditor.Utils;
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

        public UserOperations UserOperations { get; }

        public MapViewModel()
        {
            Map = new Map();
            Engine = new GameEngine();
            UserOperations = new UserOperations();
            Engine.OnFrame += OnFrameUpdate;
            _camera = new Camera();
            _gameImage = new GameImage(0,0);
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

            var op = new AddBlockOperation(Map, id.Value, index);
            UserOperations.Execute(op);
        }

        private void DeleteBlock(Point? p)
        {
            if (p == null || Map == null)
                return;

            var x = p.Value.X + _camera.Position.X;
            var y = p.Value.Y + _camera.Position.Y;

            var index   = Map.GetMapIndex(new Point(x, y));
            var blockId = Map.Blocks.GetBlockId(index.X, index.Y);

            if(blockId == null)
                return;

            var op = new DeleteBlockOperation(Map, blockId.Value, index);
            UserOperations.Execute(op);
        }

        public void GoToStartPosition()
        {
            var p = Map.Blocks.GetStartPosition();

            if (p == null)
                return;

            var size = Map.BlockSize.GetPixelSize();
            var x    = p.Value.X * size - (_gameImage.Width / 2);
            var y    = p.Value.Y * size - (_gameImage.Height / 2);

            _camera.Position = new MyPoint(x, y); ;
        }


        #region Events

        public void OnFrameUpdate()
        {
            new FrameUpdate(_gameImage, Map, _camera, _mousePosition, GetSelectedBlockID());

            RaisePropertyChanged(nameof(MapContent));
        }

        public void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var p = MyUtils.GetPosition(sender as IInputElement, e);

            if (e.ChangedButton == MouseButton.Right)
            {
                DeleteBlock(p);
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                AddBlock(p);
            }
        }

        public void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            _mousePosition = MyUtils.GetPosition(sender as IInputElement, e);

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
    
        public void OnSizeChanged(int width, int height)
        {
            if(_gameImage != null)
                _gameImage.Dispose();

           
            _gameImage = new GameImage(width, height);  // thread safe?
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

            UserOperations.Clear();

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
