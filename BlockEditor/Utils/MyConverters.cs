using BlockEditor.Models;
using LevelModel.Models;
using LevelModel.Models.Components;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace BlockEditor.Helpers
{
    public static class MyConverters
    {

        public static Blocks ToBlocks(IList<Block> pr2Blocks, out int blocksOutsideBoundries)
        {
            var blocks = new Blocks();
            blocksOutsideBoundries = 0;

            if (pr2Blocks == null)
                return blocks;

            var posX = 0;
            var posY = 0;

            foreach (var b in pr2Blocks)
            {
                posX += b.X;
                posY += b.Y;

                if(posX < 0 || posY < 0 || posX > Blocks.SIZE || posY > Blocks.SIZE)
                    blocksOutsideBoundries++;

                blocks.Add(new SimpleBlock(b.Id, posX, posY, b.Options));
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
                if (b.IsEmpty())
                    continue;

                var x     = b.Position.Value.X - previousX;
                var y     = b.Position.Value.Y - previousY;
                previousX = b.Position.Value.X;
                previousY = b.Position.Value.Y;

                var block = new Block(x, y, b.ID, b.Options);
                pr2Blocks.Add(block);
            }

            return pr2Blocks;
        }

    }
}
