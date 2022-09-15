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

    public class ArtText
    {
        public SKTextBlob TextBlob { get; private set; }
        public SKPaint Paint { get; private set; }
        public SKPoint Position { get; private set; }
        public SKPoint Scale { get; private set; }

        public ArtText(SKTextBlob blob, SKPaint paint, SKPoint position, SKPoint scale)
        {
            TextBlob = blob;
            Paint = paint;
            Position = position;
            Scale = scale;
        }
    }

    public class GameArt : IDisposable
    {
        private const string FontFamily = "Verdana";
        private const int FontSize = 18;

        public List<ArtStroke> Strokes { get; }
        public List<ArtText> Texts { get; }

        private static SKFont Font;
        private static SKTypeface Typeface;

        public GameArt(List<DrawArt> drawArts, List<TextArt> textArts)
        {
            if (Font == null)
            {
                Typeface = SKFontManager.Default.MatchFamily(FontFamily);
                Font = Typeface.ToFont(FontSize);
            }

            Strokes = CreateStrokes(drawArts);
            Texts = CreateTexts(textArts);
        }

        private static List<ArtText> CreateTexts(List<TextArt> textArts)
        {
            var texts = new List<ArtText>();
            foreach (var ta in textArts)
            {
                if (!ta.IsText)
                    continue;

                // text color comes as decimal for some reason, gotta convert to hex
                string hexcolor = "0";
                if (int.TryParse(ta.Color, out int col))
                    hexcolor = col.ToString("X");
                    
                var paint = new SKPaint
                {
                    Color = ColorUtil.GetSKColorFromRGBHex(hexcolor),
                    Typeface = Typeface,
                    TextSize = FontSize,
                };

                var position = new SKPoint(ta.X, ta.Y);
                var blob = CreateTextBlob(ta.Text, new SKPoint(), Font, paint);
                var scale = new SKPoint(ta.Width/100f, ta.Height/100f);
                
                texts.Add(new ArtText(blob, paint, position, scale));
                
            }
            return texts;
        }

        private static SKTextBlob CreateTextBlob(string text, SKPoint origin, SKFont font, SKPaint paint)
        {
            paint.GetFontMetrics(out SKFontMetrics metrics);
            var lineSpace = paint.FontSpacing + 2;
            var lines = text.Split('\r');
            var builder = new SKTextBlobBuilder();

            // there's some unknown offset flash uses at the text y origin... this isn't right but it's kinda close
            var bounds = new SKRect();
            paint.MeasureText(lines[0], ref bounds);
            origin.Offset(0, lineSpace - bounds.Height);

            foreach (var line in lines)
            {
                builder.AddRun(paint.GetGlyphs(line), font, origin);
                origin.Offset(0, lineSpace);
            }

            return builder.Build();
        }

        private static List<ArtStroke> CreateStrokes(List<DrawArt> drawArts)
        {
            var strokes = new List<ArtStroke>();
            foreach (var da in drawArts)
            {
                var paint = new SKPaint
                {
                    Color = da.IsErase ? SKColors.Transparent : ColorUtil.GetSKColorFromRGBHex(da.Color),
                    StrokeWidth = da.Size,
                    Style = SKPaintStyle.Stroke,
                    StrokeCap = SKStrokeCap.Round,
                    BlendMode = da.IsErase ? SKBlendMode.Src : SKBlendMode.SrcOver,
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
