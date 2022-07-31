﻿using BlockEditor.Helpers;
using BlockEditor.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static BlockEditor.Models.BlockImages;

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

        private GameImage _gameImage;
        private GameImage _previewImage;

        private Camera _camera;

        public BitmapImage MapContent
        {
            get => _gameImage?.CreateImage(); 
        }

        public Func<int?> GetSelectedBlockID { get; set; }

        public GameEngine Engine { get; }
        public Map Map { get; private set; }


        public MapViewModel()
        {
            Map = new Map();
            Engine = new GameEngine();
            Engine.OnFrame += OnFrame;
            Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            _camera = new Camera();
        }

        private void OnFrame()
        {
            if(Map == null || _gameImage == null || _previewImage == null)
                return;

            _gameImage.Clear(Map.Background);
            _previewImage.Clear(System.Drawing.Color.Yellow);

            var size = Map.BlockSize;

            DrawBlocks(size);
            _camera.Move(size);
            DrawSelection(size);

            RaisePropertyChanged(nameof(MapContent));
        }

        private void DrawBlocks(BlockSize size)
        {
            var width     = _gameImage.Width;
            var height    = _gameImage.Height;
            var sizeValue = size.GetPixelSize();

            var minBlockX = _camera.Position.X / sizeValue;
            var minBlockY = _camera.Position.Y / sizeValue;

            var blockCountX = width / sizeValue;
            var blockCountY = height / sizeValue;

            for (int y = minBlockY; y < minBlockY + blockCountY; y++)
            {
                for (int x = minBlockX; x < minBlockX + blockCountX; x++)
                {
                    var block = Map.Blocks.GetBlock(size, x, y);

                    if (block == null)
                        continue;

                    var posX = x * sizeValue - _camera.Position.X;
                    var posY = y * sizeValue - _camera.Position.Y;

                    _gameImage.DrawImage(ref block.Bitmap, posX, posY);
                }
            }
        }

        private void DrawSelection(BlockSize size)
        {
            if(_mousePosition == null)
                return;

            var id = GetSelectedBlockID();

            if(id == null)
                return;

            var block = BlockImages.GetImageBlock(size, id.Value);

            if(block?.Bitmap == null)
                return;

            var blockSize = size.GetPixelSize();
            var positionX = _mousePosition.Value.X - blockSize / 2;
            var positionY = _mousePosition.Value.Y - blockSize / 2;

            _gameImage.DrawImage(ref block.Bitmap, (int)positionX, (int)positionY);
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

            var x = p.Value.X * Map.BlockSizeValue - (_gameImage.Width / 2);
            var y = p.Value.Y * Map.BlockSizeValue - (_gameImage.Height / 2);

            _camera.Position = new MyPoint(x, y); ;
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

        private Point? _mousePosition;

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
            _previewImage = new GameImage(width, height); 
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

            var x = currentIndex.X * Map.BlockSizeValue - halfScreenX;
            var y = currentIndex.Y * Map.BlockSizeValue - halfScreenY;

            _camera.Position = new MyPoint(x, y);
        }

        #endregion

    }
}
