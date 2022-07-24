using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace BlockEditor.ViewModels
{

    class MapViewModel : NotificationObject
    {

        private Brush _background;
        public Brush Background
        {
            get { return _background; }
            set { RaisePropertyChanged(ref _background, value); }
        }

        private ObservableCollection<UIElement> _mapContent;
        public ObservableCollection<UIElement> MapContent
        {
            get { return _mapContent; }
            set { RaisePropertyChanged(ref _mapContent, value); }
        }


        public Func<ImageBlock> SelectedBlock { get; set; }

        private GameEngine _engine { get; }
        private Blocks _blocks { get; set; }
        private MyPoint _camera { get; set; }
        private MyPoint _mapSize;

        public MapViewModel()
        {
            _blocks = new Blocks();
            _engine = new GameEngine();
            _engine.OnFrame += OnFrame;
            _camera = new MyPoint(450 * _blocks.BlockWidth, 340 * _blocks.BlockHeight);
            Background = new SolidColorBrush(Color.FromRgb(0,0,0));
        }

        private void OnFrame()
        {
            DrawBlocks();
            MoveCamera();
        }

        private void MoveCamera()
        {
            var currentX = _camera.X;
            var currentY = _camera.Y;
            var moveStrength = 20;

            if (Keyboard.IsKeyDown(Key.Up))
                currentY -= moveStrength;

            if (Keyboard.IsKeyDown(Key.Down))
                currentY += moveStrength;

            if (Keyboard.IsKeyDown(Key.Right))
                currentX += moveStrength;

            if (Keyboard.IsKeyDown(Key.Left))
                currentX -= moveStrength;

            if (currentX < 0)
                currentX = 0;

            if (currentY < 0)
                currentY = 0;

            var maxWidth = Blocks.SIZE * _blocks.BlockWidth;
            if (currentX > maxWidth)
                currentX = maxWidth;

            var maxHeight = Blocks.SIZE * _blocks.BlockWidth;
            if (currentY > maxHeight)
                currentY = maxHeight;

            _camera = new MyPoint(currentX, currentY); // need to make thread safe?
        }

        private void DrawBlocks()
        {
            var width = _mapSize.X;
            var height = _mapSize.Y;

            var minBlockX = _camera.X / _blocks.BlockWidth;
            var minBlockY = _camera.Y / _blocks.BlockHeight;

            var blockCountX = width / _blocks.BlockWidth;
            var blockCountY = height / _blocks.BlockHeight;

            var blocks = new ObservableCollection<UIElement>();

            for (int y = minBlockY; y < minBlockY + blockCountY; y++)
            {
                for (int x = minBlockX; x < minBlockX + blockCountX; x++)
                {
                    var block = _blocks.Get(x, y);

                    if (block == null)
                        continue;

                    Canvas.SetLeft(block, x * _blocks.BlockWidth - _camera.X);
                    Canvas.SetTop(block, y * _blocks.BlockHeight - _camera.Y);
                    blocks.Add(block);
                }
            }

            MapContent = blocks;
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
            var selectedBlock = SelectedBlock?.Invoke();

            if (p == null || selectedBlock == null)
                return;

            var x = p.Value.X + _camera.X;
            var y = p.Value.Y + _camera.Y;
            var pos = new Point(x, y);

            _blocks.Add(GetMapIndex(pos), selectedBlock);
        }

        private void DeleteBlock(Point? p)
        {
            if (p == null)
                return;

            var x = p.Value.X + _camera.X;
            var y = p.Value.Y + _camera.Y;
            var pos = new Point(x, y);

            _blocks.Delete(GetMapIndex(pos));
        }

        private MyPoint GetMapIndex(Point p)
        {
            var x = (int)(p.X / _blocks.BlockWidth);
            var y = (int)(p.Y / _blocks.BlockHeight);

            return new MyPoint(x, y);
        }

        public void GoToStartPosition()
        {
            var p = _blocks.GetStartPosition();

            if(p == null)
                return;

            var x = p.Value.X * _blocks.BlockWidth  - (_mapSize.X / 2);
            var y = p.Value.Y * _blocks.BlockHeight - (_mapSize.Y / 2);
            var point = new MyPoint(x, y);

            Application.Current?.Dispatcher?.Invoke(DispatcherPriority.Render, new ThreadStart(delegate
            {
                _camera = point;
            }));
        }

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
            if (e.RightButton == MouseButtonState.Pressed)
            {
                // for permormance this is inside the if-else block
                var p = GetPosition(sender as IInputElement, e);
                DeleteBlock(p);
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (SelectedBlock?.Invoke() == null)
                    return;

                // for permormance this is inside the if-else block
                var p = GetPosition(sender as IInputElement, e);
                AddBlock(p.Value);
            }
        }

        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            _engine.Start();
        }

        public void Map_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _mapSize.X = (int)e.NewSize.Width;
            _mapSize.Y = (int)e.NewSize.Height;
        }

        internal void LoadMap(Blocks blocks)
        {
            _engine.Pause = true;
            Thread.Sleep(GameEngine.FPS * 5); // make sure engine has been stopped

            _blocks = blocks;
            GoToStartPosition();

            _engine.Pause = false;
        }
    }
}
