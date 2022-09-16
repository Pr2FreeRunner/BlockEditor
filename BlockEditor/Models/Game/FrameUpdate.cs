using BlockEditor.Helpers;
using BlockEditor.Utils;
using LevelModel.Models.Components;
using LevelModel.Models.Components.Art;

using SkiaSharp;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BlockEditor.Models
{
    class FrameUpdate
    {

        private MyPoint? _mousePosition;
        private UserSelection _userSelection;
        private Game _game;
        private SKSurface _surface;

        public FrameUpdate(Game game, SKSurface surface)
        {
            if (game?.Map == null || game?.Camera == null)
                return;

            _game = game;
            _surface = surface;
            _mousePosition = game.MousePosition;
            _userSelection = game.UserSelection;

            Update();
        }

        private void Update()
        {
            _surface.Canvas.Clear(_game.Map.Background);

            DrawArt(_game.Map.Art3, 0.25f);
            DrawArt(_game.Map.Art2, 0.5f);
            DrawArt(_game.Map.Art1, 1.0f);
            DrawBlocks();
            DrawArt(_game.Map.Art0, 1.0f);
            _game.Camera.Move(_game.Map.BlockSize);
            DrawSelectedBlock();
            DrawSelectedBlocks();
            DrawSelectedRectangle();
            DrawMeasureDistanceLine();
            DrawDebug();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void DrawDebug()
        {
            var canvas = _surface.Canvas;

            canvas.DrawText($"Camera {_game.Camera.Position}", 0, 10, MapUtil.SelectionStrokePaint);
        }

        private void DrawArt(GameArt art, float scale)
        {
            // scale is for parallax scrolling, it applies to:
            // - the draw position of objects on this layer (their speed upon moving the camera)
            // - the size of the objects on this layer

            var canvas = _surface.Canvas;
            var zoom = (float)_game.Map.BlockSize.GetScale();

            if(!_game.ShowArt)
                return;

            SKMatrix cam = SKMatrix.CreateIdentity();
            cam.TransX = -_game.Camera.Position.X;
            cam.TransY = -_game.Camera.Position.Y;
            cam.ScaleX = cam.ScaleY = zoom;
            canvas.Concat(ref cam);
            
            // draw strokes
            canvas.SaveLayer();
            foreach (var stroke in art.Strokes)
            {
                canvas.DrawPath(stroke.Path, stroke.Paint);
            }
            canvas.Restore();

            // draw texts
            foreach (var text in art.Texts)
            {
                SKMatrix textMatrix = SKMatrix.CreateIdentity();
                SKPoint projectedPosition = cam.MapPoint(text.Position);
                textMatrix.TransX = projectedPosition.X;
                textMatrix.TransY = projectedPosition.Y;
                textMatrix.ScaleX = text.Scale.X * zoom;
                textMatrix.ScaleY = text.Scale.Y * zoom;

                canvas.SetMatrix(textMatrix);

                canvas.DrawText(text.TextBlob, 0, 0, text.Paint);

                canvas.SetMatrix(cam);
            }

            SKMatrix inverted = cam.Invert();
            canvas.Concat(ref inverted);

        }

        private void DrawBlocks()
        {
            var width = _game.Camera.ScreenSize.X;
            var height = _game.Camera.ScreenSize.Y;

            var minBlockX = _game.Camera.Position.X / _game.Map.BlockPixelSize;
            var minBlockY = _game.Camera.Position.Y / _game.Map.BlockPixelSize;

            var blockCountX = width / _game.Map.BlockPixelSize;
            var blockCountY = height / _game.Map.BlockPixelSize;

            var blocks = _game.Map.Blocks;

            for (int y = minBlockY; y < minBlockY + blockCountY; y++)
            {
                for (int x = minBlockX; x < minBlockX + blockCountX; x++)
                {
                    var block = blocks.GetBlock(x, y, false);

                    if (block.IsEmpty())
                        continue;

                    DrawBlock(block, false);
                }
            }

            foreach (var startBlock in _game.Map.Blocks.StartBlocks.GetBlocks().Reverse())
            {
                if (startBlock.IsEmpty())
                    continue;

                DrawBlock(startBlock, true);
            }
        }

        private void DrawBlock(SimpleBlock b, bool semiTrans)
        {
            if (b.IsEmpty())
                return;

            BlockImage image = null;

            if (b.ID == Block.TELEPORT)
                image = BlockImages.GetTeleportImageBlock(_game.Map.BlockSize, b.Options);
            else
                image = BlockImages.GetImageBlock(_game.Map.BlockSize, b.ID);

            if (image == null)
                return;

            var posX = b.Position.Value.X * _game.Map.BlockPixelSize - _game.Camera.Position.X;
            var posY = b.Position.Value.Y * _game.Map.BlockPixelSize - _game.Camera.Position.Y;

            // TODO: better performance via DrawAtlas ?
            if (semiTrans)
                _surface.Canvas.DrawBitmap(image.SKBitmap, posX, posY, MapUtil.TranslucentPaint);
            else
                _surface.Canvas.DrawBitmap(image.SKBitmap, posX, posY);
        }

        private void DrawSelectedBlock()
        {
            if (_userSelection == null)
                return;

            if (_mousePosition == null)
                return;

            var id = BlockSelection.SelectedBlock;

            var block = BlockImages.GetImageBlock(_game.Map.BlockSize, id)?.SKBitmap;

            if (block == null)
                return;

            var positionX = (int)(_mousePosition.Value.X - _game.Map.BlockPixelSize / 2.0);
            var positionY = (int)(_mousePosition.Value.Y - _game.Map.BlockPixelSize / 2.0);

            _surface.Canvas.DrawBitmap(block, positionX, positionY, MapUtil.TranslucentPaint);
        }

        private void DrawSelectedBlocks()
        {
            if (_userSelection == null)
                return;

            if (_mousePosition == null)
                return;

            var blocks = BlockSelection.SelectedBlocks;

            if (blocks == null)
                return;

            var arrayX = (int)(_mousePosition.Value.X + _game.Map.BlockPixelSize / 2.0);
            var arrayY = (int)(_mousePosition.Value.Y + _game.Map.BlockPixelSize / 2.0);

            var width  = ArrayUtil.GetMaxWidth(blocks);
            var height = ArrayUtil.GetMaxHeight(blocks);

            foreach(var b in blocks)
            {
                if (b.IsEmpty())
                    continue;

                var block = BlockImages.GetImageBlock(_game.Map.BlockSize, b.ID)?.SKBitmap;

                if (block == null)
                    continue;

                var blockX = arrayX - width  * _game.Map.BlockPixelSize + b.Position.Value.X * _game.Map.BlockPixelSize - _game.Map.BlockPixelSize;
                var blockY = arrayY - height * _game.Map.BlockPixelSize + b.Position.Value.Y * _game.Map.BlockPixelSize - _game.Map.BlockPixelSize;

                _surface.Canvas.DrawBitmap(block, blockX, blockY);
            }
        }

        private void DrawSelectedRectangle()
        {
            if (_userSelection == null)
                return;

            var start = _userSelection.ImageRegion?.Start;
            var end = _userSelection.ImageRegion?.End ?? _mousePosition;

            if (start == null || end == null)
                return;

            var minX = Math.Min(start.Value.X, end.Value.X);
            var minY = Math.Min(start.Value.Y, end.Value.Y);
            var maxX = Math.Max(start.Value.X, end.Value.X);
            var maxY = Math.Max(start.Value.Y, end.Value.Y);

            var rec = new SKRect(minX, minY, maxX, maxY);

            _surface.Canvas.DrawRect(rec, MapUtil.SelectionFillPaint);
            _surface.Canvas.DrawRect(rec, MapUtil.SelectionStrokePaint);
        }

        private void DrawMeasureDistanceLine()
        {
            if (_game.MeasureDistance.ImagePoint1 == null)
                return;

            if (_game.MeasureDistance.MapPoint1 == null)
                return;

            var start = _game.MeasureDistance.ImagePoint1;
            var end = _game.MeasureDistance.ImagePoint2 ?? _mousePosition;

            if (start == null || end == null)
                return;

            var p1 = new SKPoint(start.Value.X, start.Value.Y);
            var p2 = new SKPoint(end.Value.X, end.Value.Y);

            _surface.Canvas.DrawLine(p1, p2, MapUtil.SelectionStrokePaint);
        }

    }
}
