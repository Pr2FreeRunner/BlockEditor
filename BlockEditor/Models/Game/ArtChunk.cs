using SkiaSharp;

namespace BlockEditor.Models
{
    /// <summary>
    /// A rasterized section of art for stamping on a SKCanvas quickly.
    /// </summary>
    internal class ArtChunk
    {
        public SKRect Region { get; set; }
        public SKImage Image { get; set; }

        public ArtChunk(SKRect region, SKImage image)
        {
            Region = region;
            Image = image;
        }
    }
}
