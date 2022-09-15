using BlockEditor.Helpers;
using BlockEditor.Utils;

using LevelModel.Models.Components.Art;

using SkiaSharp;

using System;
using System.Collections.Generic;
using System.Text;

namespace BlockEditor.Models
{
    public class MyStrokeArt
    {
        public SKPath Path { get; private set; }
        public SKPaint Paint { get; private set; }

        public MyStrokeArt(SKPath path, SKPaint paint)
        {
            Path = path;
            Paint = paint;
        }
    }

    public class MyTextArt
    {
        public SKTextBlob TextBlob { get; private set; }
        public SKPaint Paint { get; private set; }
        public SKPoint Position { get; private set; }
        public SKPoint Scale { get; private set; }

        public MyTextArt(SKTextBlob blob, SKPaint paint, SKPoint position, SKPoint scale)
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

        public List<MyStrokeArt> Strokes { get; }
        public List<MyTextArt> Texts { get; }

        private static SKFont Font;
        private static SKTypeface Typeface;
        private readonly string _name;

        public GameArt(string name)
        {
            if (Font == null)
            {
                Typeface = SKFontManager.Default.MatchFamily(FontFamily);
                Font = Typeface.ToFont(FontSize);
            }

            _name = name ?? "art";
            Strokes = new List<MyStrokeArt>();
            Texts = new List<MyTextArt>();
        }


        public void LoadArt(List<DrawArt> drawArts, List<TextArt> textArts)
        {
            try
            {
                Strokes.Clear();
                Texts.Clear();

                LoadStrokes(drawArts);
                LoadTexts(textArts);
            }
            catch
            {
                MessageUtil.ShowError($"Failed to load the {_name}" + Environment.NewLine + Environment.NewLine +"The art inside the map will be ignored.");
            }
        }

        private void LoadTexts(List<TextArt> textArts)
        {
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

                Texts.Add(new MyTextArt(blob, paint, position, scale));
            }
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

        private void LoadStrokes(List<DrawArt> drawArts)
        {
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
                
                Strokes.Add(new MyStrokeArt(path, paint));
            }
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
