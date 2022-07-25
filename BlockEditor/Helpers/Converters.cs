using BlockEditor.Models;
using LevelModel.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace BlockEditor.Helpers
{
    public static class MyConverters
    {

        public static bool TryParse(string input, out int result)
        {
            return int.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        public static Blocks ToBlocks(Level level)
        {
            var blocks = new Blocks();

            if (level?.Blocks == null)
                return blocks;

            var posX = 0;
            var posY = 0;

            foreach (var b in level.Blocks)
            {
                posX += b.X;
                posY += b.Y;

                var pos = new MyPoint(posX, posY);

                if (BlockImages.Images.TryGetValue(b.Id, out var image))
                    blocks.Add(pos, image);
                else
                    Debugger.Break();
            }

            return blocks;
        }
    }
}
