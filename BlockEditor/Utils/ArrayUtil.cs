using BlockEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockEditor.Utils
{
    public class ArrayUtil
    {

        public static int GetMaxSize(List<SimpleBlock> blocks)
        {
            if(!blocks.AnyBlocks())
                return 0;

            var height = GetMaxHeight(blocks);
            var width = GetMaxWidth(blocks);

            return Math.Max(width, height);
        }

        public static int GetMaxWidth(List<SimpleBlock> blocks)
        {
            if (!blocks.AnyBlocks())
                return 0;

            return blocks.Max(b => b.Position?.X ?? 0);
        }

        public static int GetMaxHeight(List<SimpleBlock> blocks)
        {
            if (!blocks.AnyBlocks())
                return 0;

            return blocks.Max(b => b.Position?.Y ?? 0);
        }

        public static List<SimpleBlock> RotateRight(List<SimpleBlock> blocks)
        {
            if (!blocks.AnyBlocks())
                return blocks;

            var size = GetMaxSize(blocks);

            return blocks.RemoveEmpty().Select(b => b.Move(size - b.Position.Value.Y - 1, b.Position.Value.X)).ToList();
        }


        public static List<SimpleBlock> VerticalFlip(List<SimpleBlock> blocks)
        {
            if (!blocks.AnyBlocks())
                return blocks;

            var size = GetMaxSize(blocks);

            return blocks.RemoveEmpty().Select(b => b.Move(b.Position.Value.X, size - b.Position.Value.Y - 1)).ToList();
        }

        public static List<SimpleBlock> HorizontalFlip(List<SimpleBlock> blocks)
        {
            if (!blocks.AnyBlocks())
                return blocks;

            var size = GetMaxSize(blocks);

            return blocks.RemoveEmpty().Select(b => b.Move(size - b.Position.Value.X - 1,b.Position.Value.Y)).ToList();
        }
    }
}
