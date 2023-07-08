using BlockEditor.Helpers;
using BlockEditor.Utils;
using Converters;
using Converters.DataStructures.DTO;
using LevelModel.Models;
using SkiaSharp;
using System;

using static BlockEditor.Models.BlockImages;

namespace BlockEditor.Models
{
    public class Map : IDisposable
    {

        public Level Level { get; }

        public Blocks Blocks { get; }
        public GameArt Art0 { get; }
        public GameArt Art1 { get; }

        public MapRenderer Renderer { get; }


        public readonly int BlocksOutsideBoundries;

        public SKColor Background
        {
            get
            {
                try
                {
                    if (Level.BackgroundColor != null)
                        return ColorUtil.GetSKColorFromRGBHex(Level.BackgroundColor);
                    else
                        return SKColors.Black;
                }
                catch
                {
                    return SKColors.Black;
                }
            }
        }

        public int BlockPixelSize;
        private BlockSize _blockSize;

        public BlockSize BlockSize
        {
            get
            {
                return _blockSize;
            }
            set
            {
                _blockSize = value;
                BlockPixelSize = value.GetPixelSize();
            }
        }

        public Map(Level level = null)
        {
            Renderer = new MapRenderer();
            Level = level ?? GetDefaultLevel();
            Blocks = BlocksUtil.ToBlocks(Level.Blocks, out BlocksOutsideBoundries);
            BlockSize = DEFAULT_BLOCK_SIZE;
            Art0 = new GameArt("Art 0");
            Art1 = new GameArt("Art 1");

            ArtUtil.CreateAbsolutePosition(Level.TextArt0);
            ArtUtil.CreateAbsolutePosition(Level.TextArt1);

            LoadArt();
        }

        public void LoadArt()
        {
            Renderer.ClearCache(Art0);
            Renderer.ClearCache(Art1);

            Art0.LoadArt(Level.DrawArt0, Level.TextArt0);
            Art1.LoadArt(Level.DrawArt1, Level.TextArt1);
        }


        public string ToPr2String(string username, string token, bool publish, bool overwrite, bool newest)
        {
            ArtUtil.CreateRelativePosition(Level.TextArt0);
            ArtUtil.CreateRelativePosition(Level.TextArt1);

            try 
            { 
                Level.Blocks = BlocksUtil.ToPr2Blocks(Blocks);
                Level.Published = publish;

                if (username == null || token == null)
                    throw new ArgumentNullException("user");

                var dto = new ToPr2DTO()
                {
                    Level = Level,
                    Username = username,
                    Token = token,
                    OverWrite = overwrite,
                    Newest = newest
                };

                return PR2Converter.LevelToPr2(dto);
            }
            finally
            {
                ArtUtil.CreateAbsolutePosition(Level.TextArt0);
                ArtUtil.CreateAbsolutePosition(Level.TextArt1);
            }
        }

        private Level GetDefaultLevel()
        {
            var data = "version=1&credits=&cowboyChance=0&title=&time=1658836801&note=&min_level=0&song=&gravity=1.0&max_time=0&has_pass=0&live=0"
                     + "&items=1&gameMode=race&badHats=&data=m4`0`444;335;111,1;0;112,1;0;113,1;0;114```````-1````4fde8f5661b3bc371330feb1b59eeb33";

            var levelInfo = Parsers.PR2Parser.Level(data);

            return levelInfo.Level;
        }

        public MyPoint GetMapIndex(MyPoint p)
        {
            var x = p.X / BlockPixelSize;
            var y = p.Y / BlockPixelSize;

            return new MyPoint(x, y);
        }

        #region IDisposable

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Art0.Dispose();
                    Art1.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
