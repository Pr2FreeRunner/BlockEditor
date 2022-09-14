using BlockEditor.Utils;

using LevelModel.Models.Components.Art;

using SkiaSharp;

using System;
using System.Collections.Generic;
using System.Text;

namespace BlockEditor.Models
{
    public class ArtStroke
    {
        public SKPath Path { get; private set; }
        public SKPaint Paint { get; private set; }

        public ArtStroke(SKPath path, SKPaint paint)
        {
            Path = path;
            Paint = paint;
        }
    }

    public class ArtGraphics : IDisposable
    {
        public List<ArtStroke> Strokes { get; }

        public ArtGraphics(List<DrawArt> drawArts)
        {
            Strokes = CreateStrokes(drawArts);
        }

        private static List<ArtStroke> CreateStrokes(List<DrawArt> drawArts)
        {
            var strokes = new List<ArtStroke>();
            foreach (var da in drawArts)
            {
                var paint = new SKPaint
                {
                    Color = ColorUtil.GetSKColorFromRGBHex(da.Color),
                    StrokeWidth = da.Size,
                    Style = SKPaintStyle.Stroke,
                };
                var path = new SKPath();
                path.MoveTo(da.X, da.Y);
                for (int i = 0; i < da.Movement.Count; i += 2)
                {
                    path.RLineTo(da.Movement[i], da.Movement[i + 1]);
                }
                strokes.Add(new ArtStroke(path, paint));
            }
            return strokes;
        }


        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var stroke in Strokes)
                    {
                        stroke.Paint.Dispose();
                        stroke.Path.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Art()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
