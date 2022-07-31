using BlockEditor.Models;
using LevelModel.Models;
using LevelModel.Models.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using static BlockEditor.Models.BlockImages;

namespace BlockEditor.Helpers
{
    public static class MyConverters
    {

        public static bool TryParse(string input, out int result)
        {
            return int.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        public static Blocks ToBlocks(IList<Block> pr2Blocks)
        {
            var blocks = new Blocks();

            if (pr2Blocks == null)
                return blocks;

            var posX = 0;
            var posY = 0;

            foreach (var b in pr2Blocks)
            {
                posX += b.X;
                posY += b.Y;

                var pos = new MyPoint(posX, posY);

                blocks.Add(pos, b.Id);
            }

            return blocks;
        }

        public static List<Block> ToPr2Blocks(Blocks blocks)
        {
            var pr2Blocks = new List<Block>();

            if (blocks == null)
                return pr2Blocks;

            var previousX = 0;
            var previousY = 0;
            var allBlocks = blocks.StartBlocks.GetBlocks().Concat(blocks.GetBlocks());

            foreach (var b in allBlocks)
            {
                if (b?.Position == null)
                    continue;

                var x     = b.Position.Value.X - previousX;
                var y     = b.Position.Value.Y - previousY;
                previousX = b.Position.Value.X;
                previousY = b.Position.Value.Y;

                var block = new Block(x, y, b.ID, string.Empty);
                pr2Blocks.Add(block);
            }

            return pr2Blocks;
        }
    }
}
