using BlockEditor.Helpers;
using System;
using System.Drawing;

namespace BlockEditor.Models
{
    class FrameUpdate
    {

        private MyPoint? _mousePosition;
        private int? _selectedBlockID;
        private Graphics _graphics;
        private Game _game;

        public FrameUpdate(Game game, MyPoint? mousePosition, int? selectedBlockID)
        {
            if (game?.GameImage == null || game?.Map == null || game?.Camera == null)
                return;

            _game = game;
            _mousePosition = mousePosition;
            _selectedBlockID = selectedBlockID;
            _graphics = CreateGraphics();

            Update();
        }

        private void Update()
        {
            _game.GameImage.Clear(_game.Map.Background);

            //DrawGrids();
            DrawBlocks();
            _game.Camera.Move(_game.Map.BlockSize);
            DrawSelection();
        }

        private Graphics CreateGraphics()
        {
            var bmp = _game.GameImage?.GetBitmap();
            var ex  = new Exception("Failed to generate graphics for the game");

            if (bmp == null)
                throw ex;

            return Graphics.FromImage(bmp) ?? throw ex;
        }

        private void DrawGrids()
        {
            var width  = _game.GameImage.Width;
            var height = _game.GameImage.Height;
            var startX = _game.Camera.Position.X / _game.Map.BlockPixelSize / 30;
            var startY = _game.Camera.Position.X / _game.Map.BlockPixelSize / 30;
            var pencil = MapUtil.GetGridPen(_game.Map.Background);

            for (int i = startX; i < width; i += _game.Map.BlockPixelSize)
                _graphics.DrawLine(pencil, i, 0, i, height);


            for (int i = startY; i < height; i += _game.Map.BlockPixelSize)
                _graphics.DrawLine(pencil, 0, i, width, i);

        }

        private void DrawBlocks()
        {
            var width  = _game.GameImage.Width;
            var height = _game.GameImage.Height;

            var minBlockX = _game.Camera.Position.X / _game.Map.BlockPixelSize;
            var minBlockY = _game.Camera.Position.Y / _game.Map.BlockPixelSize;

            var blockCountX = width  / _game.Map.BlockPixelSize;
            var blockCountY = height / _game.Map.BlockPixelSize;

            for (int y = minBlockY; y < minBlockY + blockCountY; y++)
            {
                for (int x = minBlockX; x < minBlockX + blockCountX; x++)
                {
                    var id = _game.Map.Blocks.GetBlockId(x, y);

                    DrawBlock(id, x, y, false);
                }
            }

            foreach (var startBlock in _game.Map.Blocks.StartBlocks.GetBlocks())
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

            var block = BlockImages.GetImageBlock(_game.Map.BlockSize, id.Value);

            if (block == null)
                return;

            var posX = x * _game.Map.BlockPixelSize - _game.Camera.Position.X;
            var posY = y * _game.Map.BlockPixelSize - _game.Camera.Position.Y;

            if(semiTrans)
                _game.GameImage.DrawTransperentImage(_graphics, block.SemiTransparentBitmap, posX, posY);
            else
                _game.GameImage.DrawImage(ref block.Bitmap, posX, posY);
        }

        private void DrawSelection()
        {
            if (_mousePosition == null)
                return;

            var id = _selectedBlockID;

            if (id == null)
                return;

            var block = BlockImages.GetImageBlock(_game.Map.BlockSize, id.Value)?.SemiTransparentBitmap;

            if (block == null)
                return;

            var positionX = (int) (_mousePosition.Value.X - _game.Map.BlockPixelSize / 2.0);
            var positionY = (int) (_mousePosition.Value.Y - _game.Map.BlockPixelSize / 2.0);

            _game.GameImage.DrawTransperentImage(_graphics, block, positionX, positionY);
        }

    }
}
