using BlockEditor.Helpers;
using BlockEditor.Utils;
using Converters;
using Converters.DataStructures.DTO;
using LevelModel.Models;
using System;
using System.Collections.Generic;
using System.Drawing;

using static BlockEditor.Models.BlockImages;

namespace BlockEditor.Models
{
    public class Map
    {

        public Level Level { get; }

        public Blocks Blocks { get; }

        public Color Background
        {
            get
            {
                try 
                { 
                    if (Level.BackgroundColor != null)
                        return ColorTranslator.FromHtml("#" + Level.BackgroundColor);
                    else
                        return Color.Black;
                }
                catch
                {
                    return Color.Black;
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


        public Map()
        {
            Level  = GetDefaultLevel();
            Level.Title = string.Empty;
            Blocks    = MyConverters.ToBlocks(Level.Blocks);
            BlockSize = BlockImages.DEFAULT_BLOCK_SIZE;
        }

        public Map(Level level)
        {
            Level = level ?? GetDefaultLevel();
            Level.Title = Level?.Title ?? string.Empty;
            Blocks = MyConverters.ToBlocks(Level.Blocks);
        }


        public string ToPr2String(string username, string token, bool publish = false, bool overwrite = false)
        {
            Level.Blocks = MyConverters.ToPr2Blocks(Blocks);
            Level.Published = publish;

            if (username == null || token == null)
                throw new ArgumentNullException("user");

            var dto = new ToPr2DTO()
            {
                Level = Level,
                Username = username,
                Token = token,
                OverWrite = overwrite
            };

            return PR2Converter.LevelToPr2(dto);
        }

        private Level GetDefaultLevel()
        {
            var data = "level_id=6510271&version=2&user_id=2672882&credits=&cowboyChance=0&title=Default&time=1658836801&note=&min_level=0&song=&gravity=1.0&max_time=0&has_pass=0&live=1"
                     +"&items=1&gameMode=race&badHats=&data=m4`0`444;335;111,1;0;112,1;0;113,1;0;114```````-1````4fde8f5661b3bc371330feb1b59eeb33";

            var levelInfo = Parsers.PR2Parser.Level(data);

            return levelInfo.Level;
        }

        public MyPoint GetMapIndex(MyPoint p)
        {
            var x = (int)(p.X / BlockPixelSize);
            var y = (int)(p.Y / BlockPixelSize);

            return new MyPoint(x, y);
        }

    }
}
