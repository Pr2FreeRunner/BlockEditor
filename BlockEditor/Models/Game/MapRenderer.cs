using BlockEditor.Helpers;
using BlockEditor.Utils;

using LevelModel.Models.Components;

using SkiaSharp;

using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockEditor.Models
{
    public class MapRenderer : IDisposable
    {
        private const int ChunkPixelSize = 1024;
        private readonly Dictionary<GameArt, List<ArtChunk>> _artChunksCache = new Dictionary<GameArt, List<ArtChunk>>();
        private bool _disposedValue;

        public void ClearCache(GameArt art)
        {
            if (_artChunksCache.TryGetValue(art, out var chunks))
            {
                _artChunksCache.Remove(art);
                foreach (var chunk in chunks)
                {
                    chunk.Image.Dispose();
                }
            }
        }

        public void RenderBlocks(SKCanvas canvas, Game game, IEnumerable<SimpleBlock> blocks)
        {
            var sprites = new SKRect[blocks.Count()];
            var transforms = new SKRotationScaleMatrix[blocks.Count()];
            var colors = new SKColor[blocks.Count()];

            var i = 0;
            foreach (var block in blocks)
            {
                sprites[i] = BlockImages.GetSpriteFromId(block.ID);

                var matrix = SKRotationScaleMatrix.CreateScale((float)BlockSizeUtil.GetScale(game.Map.BlockSize));
                matrix.TX = block.Position.Value.X * game.Map.BlockPixelSize - game.Camera.Position.X;
                matrix.TY = block.Position.Value.Y * game.Map.BlockPixelSize - game.Camera.Position.Y;
                transforms[i] = matrix;

                colors[i] = block.ID == Block.TELEPORT ? new SKColor(BlocksUtil.GetTeleportColor(block)) : SKColors.Transparent;

                i++;
            }

            using var paint = new SKPaint
            {
                IsAntialias = false,
                FilterQuality = SKFilterQuality.Low
            };

            canvas.DrawAtlas(BlockImages.BlocksSheet, sprites, transforms, colors, SKBlendMode.SrcOver, paint);
        }

        private List<ArtChunk> CreateArtCache(GameArt art)
        {
            var chunks = new List<ArtChunk>();
            var strokeBounds = art.Strokes.Select(s => s.Path.Bounds);
            var slicer = new ArtChunkSlicer(strokeBounds, ChunkPixelSize);

            foreach (var region in slicer.ChunkRegions)
            {
                using var surface = SKSurface.Create(new SKImageInfo((int)region.Width, (int)region.Height, SKColorType.Bgra8888, SKAlphaType.Premul));
                if (surface is null) return null;

                var canvas = surface.Canvas;
#if DEBUG
                using var darkText = new SKPaint { Color = SKColors.Black.WithAlpha(200), IsAntialias = true };
                using var lightText = new SKPaint { Color = SKColors.White.WithAlpha(200), IsAntialias = true };
                canvas.DrawText($"chunk {region.Location}", new SKPoint(10, 10), lightText);
                canvas.DrawText($"chunk {region.Location}", new SKPoint(12, 12), darkText);
#endif

                canvas.SaveLayer();
                var offsetMatrix = SKMatrix.CreateTranslation(-region.Location.X, -region.Location.Y);
                canvas.SetMatrix(canvas.TotalMatrix.PostConcat(offsetMatrix));
                foreach (var stroke in art.Strokes)
                {
                    canvas.DrawPath(stroke.Path, stroke.Paint);
                }
                canvas.Restore();

                var chunk = new ArtChunk(region, image: surface.Snapshot());
                chunks.Add(chunk);
            }
            
            return chunks;
        }

        public void RenderArt(SKCanvas canvas, GameArt art)
        {
            if (!_artChunksCache.ContainsKey(art))
            {
                var cache = CreateArtCache(art);
                if (cache != null)
                    _artChunksCache[art] = cache;
            }

            if (_artChunksCache.TryGetValue(art, out var chunks))
            {
                foreach (var chunk in chunks)
                {
                    canvas.DrawImage(chunk.Image, chunk.Region.Location);
                }
            }

            //canvas.SaveLayer();
            //var offsetMatrix = SKMatrix.CreateTranslation(-region.Location.X, -region.Location.Y);
            //canvas.SetMatrix(canvas.TotalMatrix.PostConcat(offsetMatrix));
            //foreach (var stroke in strokes)
            //{
            //    canvas.DrawPath(stroke.Path, stroke.Paint);
            //}
            //canvas.Restore();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (var art in _artChunksCache.Keys)
                    {
                        ClearCache(art);
                    }
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
