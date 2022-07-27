using BlockEditor.Helpers;
using Converters;
using Converters.DataStructures.DTO;
using LevelModel.Models;
using System;

namespace BlockEditor.Models
{
    public class Map
    {


        public Blocks Blocks { get; set; }

        public string Title { 
            get => _backend.Title;
            set => _backend.Title = value;
        }


        private readonly Level _backend;

        public Map()
        {
            _backend = GetDefaultLevel();
            Title = string.Empty;
            Blocks = MyConverters.ToBlocks(_backend.Blocks);
        }

        public Map(Level level)
        {
            _backend = level ?? GetDefaultLevel();
            Title = _backend?.Title ?? string.Empty;
            Blocks = MyConverters.ToBlocks(_backend.Blocks);
        }

        public string ToPr2String(string username, string token, bool overwrite)
        {
            _backend.Blocks = MyConverters.ToPr2Blocks(Blocks);
            _backend.Published = false;

            if (username == null || token == null)
                throw new ArgumentNullException("user");

            var dto = new ToPr2DTO()
            {
                Level = _backend,
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


    }
}
