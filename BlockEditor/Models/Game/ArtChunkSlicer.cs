using SkiaSharp;

using System;
using System.Collections.Generic;

namespace BlockEditor.Models
{
    /// <summary>
    /// Computes ArtChunk regions that art objects will occupy.
    /// </summary>
    internal class ArtChunkSlicer
    {
        public ISet<SKRect> ChunkRegions { get; private set; }

        private readonly int _chunkSize;

        public ArtChunkSlicer(IEnumerable<SKRect> artObjectBounds, int chunkSize)
        {
            _chunkSize = chunkSize;
            SliceChunks(artObjectBounds);
        }

        private void SliceChunks(IEnumerable<SKRect> objs)
        {
            ChunkRegions = new HashSet<SKRect>();

            foreach (var chunkable in objs)
            {
                var regions = FindChunkRegionsThatArtObjectOccupies(chunkable);
                foreach (var region in regions)
                    if (!ChunkRegions.Contains(region))
                        ChunkRegions.Add(region);
            }
        }

        private IEnumerable<SKRect> FindChunkRegionsThatArtObjectOccupies(SKRect artObject)
        {
            var minX = (int) Math.Floor(artObject.Left / _chunkSize) * _chunkSize;
            var minY = (int) Math.Floor(artObject.Top / _chunkSize) * _chunkSize;
            var maxX = (int) Math.Ceiling(artObject.Right / _chunkSize) * _chunkSize;
            var maxY = (int) Math.Ceiling(artObject.Bottom / _chunkSize) * _chunkSize;

            for (int y = minY; y < maxY; y += _chunkSize)
            {
                for (int x = minX; x < maxX; x += _chunkSize)
                {
                    yield return new SKRect(x, y, x + _chunkSize, y + _chunkSize);
                }
            }
        }
    }
}
