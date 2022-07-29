using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Views;
using DataAccess.DataStructures;
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
    public class MapViewModel : NotificationObject
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

        public GameEngine Engine { get; }
        public Map Map { get; private set; }

        private Camera _camera { get; set; }
        private MyPoint _mapSize;

        public MapViewModel()
        {
            Map = new Map();
            Engine = new GameEngine();
            Engine.OnFrame += OnFrame;
            Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }

        private void OnFrame()
        {
            DrawBlocks();
            _camera = _camera.Move(Map.Blocks.BlockWidth, Map.Blocks.BlockHeight);
        }

        private void DrawBlocks()
        {
            var width = _mapSize.X;
            var height = _mapSize.Y;

            var minBlockX = _camera.Position.X / Map.Blocks.BlockWidth;
            var minBlockY = _camera.Position.Y / Map.Blocks.BlockHeight;

            var blockCountX = width / Map.Blocks.BlockWidth;
            var blockCountY = height / Map.Blocks.BlockHeight;

            var blocks = new ObservableCollection<UIElement>();

            for (int y = minBlockY; y < minBlockY + blockCountY; y++)
            {
                for (int x = minBlockX; x < minBlockX + blockCountX; x++)
                {
                    var block = Map.Blocks.Get(x, y);

                    if (block == null)
                        continue;

                    Canvas.SetLeft(block, x * Map.Blocks.BlockWidth - _camera.Position.X);
                    Canvas.SetTop(block, y * Map.Blocks.BlockHeight - _camera.Position.Y);
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

            if (p == null || selectedBlock == null || Map == null)
                return;

            var x = p.Value.X + _camera.Position.X;
            var y = p.Value.Y + _camera.Position.Y;
            var pos = new Point(x, y);

            Map.Blocks.Add(Map.GetMapIndex(pos), selectedBlock);
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

            var x = p.Value.X * Map.Blocks.BlockWidth - (_mapSize.X / 2);
            var y = p.Value.Y * Map.Blocks.BlockHeight - (_mapSize.Y / 2);
            var point = new MyPoint(x, y);

            Application.Current?.Dispatcher?.Invoke(DispatcherPriority.Render, new ThreadStart(delegate
            {
                _camera = new Camera(point);
            }));
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
    
        public void Map_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _mapSize.X = (int)e.NewSize.Width;
            _mapSize.Y = (int)e.NewSize.Height;
        }
       
        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            Engine.Start();
        }

        internal void OnZoomChanged(double obj)
        {

        }

        internal void OnLoadMap(Map map)
        {
            if (map == null)
                return;

            Engine.Pause = true;
            Thread.Sleep(GameEngine.FPS * 5); // make sure engine has been stopped

            Map = map;
            GoToStartPosition();

            Engine.Pause = false;
        }

        #endregion

    }
}
