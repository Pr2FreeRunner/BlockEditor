using BlockEditor.Helpers;
using System;
using System.Drawing;

namespace BlockEditor.Models
{
    class FrameUpdater
    {

        private GameImage _gameImage;
        private Map _map;
        private Camera _camera;
        private MyPoint? _mousePosition;
        private int? _selectedBlockID;
        private Graphics _graphics;

        public FrameUpdater(GameImage image, Map map, Camera camera, MyPoint? mousePosition, int? selectedBlockID)
        {
            if (image == null || map == null || camera == null)
                return;

            _gameImage = image;
            _map = map;
            _camera = camera;
            _mousePosition = mousePosition;
            _selectedBlockID = selectedBlockID;
            _graphics = CreateGraphics();

            Update();
        }

        private void Update()
        {
            _gameImage.Clear(_map.Background);

            //DrawGrids();
            DrawBlocks();
            _camera.Move(_map.BlockSize);
            DrawSelection();
        }

        private Graphics CreateGraphics()
        {
            var bmp = _gameImage?.GetBitmap();
            var ex  = new Exception("Failed to generate graphics for the game");

            if (bmp == null)
                throw ex;

            return Graphics.FromImage(bmp) ?? throw ex;
        }

        private void DrawGrids()
        {
            var width  = _gameImage.Width;
            var height = _gameImage.Height;
            var startX = _camera.Position.X / _map.BlockPixelSize / 30;
            var startY = _camera.Position.X / _map.BlockPixelSize / 30;
            var pencil = MapUtil.GetGridPen(_map.Background);

            for (int i = startX; i < width; i += _map.BlockPixelSize)
                _graphics.DrawLine(pencil, i, 0, i, height);


            for (int i = startY; i < height; i += _map.BlockPixelSize)
                _graphics.DrawLine(pencil, 0, i, width, i);

        }

        private void DrawBlocks()
        {
            var width  = _gameImage.Width;
            var height = _gameImage.Height;

            var minBlockX = _camera.Position.X / _map.BlockPixelSize;
            var minBlockY = _camera.Position.Y / _map.BlockPixelSize;

            var blockCountX = width  / _map.BlockPixelSize;
            var blockCountY = height / _map.BlockPixelSize;

            for (int y = minBlockY; y < minBlockY + blockCountY; y++)
            {
                for (int x = minBlockX; x < minBlockX + blockCountX; x++)
                {
                    var id = _map.Blocks.GetBlockId(x, y);

                    DrawBlock(id, x, y, false);
                }
            }

            foreach (var startBlock in _map.Blocks.StartBlocks.GetBlocks())
            {
                if (startBlock?.Position == null)
                    continue;

                var x = startBlock.Position.Value.X;
                var y = startBlock.Position.Value.Y;

                DrawBlock(startBlock.ID, x, y, true);
            }
        }

        private void DrawBlock(int? id, int x, int y, bool semiTrans)
        {
            if(id == null)
                return;

            var block = BlockImages.GetImageBlock(_map.BlockSize, id.Value);

            if (block == null)
                return;

            var posX = x * _map.BlockPixelSize - _camera.Position.X;
            var posY = y * _map.BlockPixelSize - _camera.Position.Y;

            if(semiTrans)
                _gameImage.DrawTransperentImage(_graphics, block.SemiTransparentBitmap, posX, posY);
            else
                _gameImage.DrawImage(ref block.Bitmap, posX, posY);
        }

        private void DrawSelection()
        {
            if (_mousePosition == null)
                return;

            var id = _selectedBlockID;

            if (id == null)
                return;

            var block = BlockImages.GetImageBlock(_map.BlockSize, id.Value)?.SemiTransparentBitmap;

            if (block == null)
                return;

            var positionX = (int) (_mousePosition.Value.X - _map.BlockPixelSize / 2.0);
            var positionY = (int) (_mousePosition.Value.Y - _map.BlockPixelSize / 2.0);

            _gameImage.DrawTransperentImage(_graphics, block, positionX, positionY);
        }

    }
}
