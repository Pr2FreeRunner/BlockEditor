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
    public class Map
    {

        public Level Level { get; }

        public Blocks Blocks { get; }
        public GameArt Art1 { get; }
        public GameArt Art2 { get; }
        public GameArt Art3 { get; }

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
            Level = level ?? GetDefaultLevel();
            Blocks = BlocksUtil.ToBlocks(Level.Blocks, out BlocksOutsideBoundries);
            BlockSize = DEFAULT_BLOCK_SIZE;
            Art1 = new GameArt("Art 1");
            Art2 = new GameArt("Art 2");
            Art3 = new GameArt("Art 3");

            LoadArt();
        }

        public void LoadArt()
        {
            Art1.LoadArt(Level.DrawArt1, Level.TextArt1);
            Art2.LoadArt(Level.DrawArt2, Level.TextArt2);
            Art3.LoadArt(Level.DrawArt3, Level.TextArt3);
        }


        public string ToPr2String(string username, string token, bool publish, bool overwrite, bool newest)
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

        private Level GetDefaultLevel()
        {
            var data = "version=1&credits=&cowboyChance=0&title=&time=1658836801&note=&min_level=0&song=&gravity=1.0&max_time=0&has_pass=0&live=1"
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

    }
}
