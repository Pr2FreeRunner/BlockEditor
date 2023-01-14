using BlockEditor.Models;
using BlockEditor.Utils;
using BlockEditor.Views.Windows;

using LevelModel.Models.Components;

using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockEditor.Helpers
{
    public static class BlocksUtil
    {
        public static uint GetTeleportColor(SimpleBlock block)
        {
            try
            {
                return 0xff000000 + (block.Options == "" ? 0 : uint.Parse(block.Options));
            }
            catch
            {
                return 0;
            }
        }

        public static List<SimpleBlock> ReplaceBlock(Blocks blocks, List<int> replace, List<int> add, MyRegion region)
        {
            var result = new List<SimpleBlock>();
            var notFound = -1;

            if (blocks == null || replace == null || add == null || replace.Count != add.Count)
                return result;

            foreach(var b in GetBlocks(blocks, region))
            {
                if(b.IsEmpty())
                    continue;

                var index = replace.IndexOf(b.ID);

                if (index != notFound)
                    result.Add(new SimpleBlock(add[index], b.Position.Value.X, b.Position.Value.Y));
            }

            return result;
        }

        public static List<SimpleBlock> GetBlocks(Blocks blocks, MyRegion region)
        {
            var result = new List<SimpleBlock>();

            if (blocks == null)
                return result;

            var lowerLimit = new MyPoint(0, 0);
            var upperLimit = new MyPoint(Blocks.SIZE, Blocks.SIZE);

            if (region.IsComplete())
            {
                lowerLimit = region.Start.Value;
                upperLimit = region.End.Value;
            }

            for (int x = lowerLimit.X; x < upperLimit.X; x++)
            {
                for (int y = lowerLimit.Y; y < upperLimit.Y; y++)
                {
                    result.Add(blocks.GetBlock(x, y, false));
                    result.AddRange(blocks.StartBlocks.GetBlocks(x, y));
                }
            }


            return result;
        }

        public static List<SimpleBlock> RemoveBlocks(Blocks blocks, List<int> ids, MyRegion region)
        {
            var result = new List<SimpleBlock>();

            if (blocks == null || ids == null)
                return result;

            foreach(var b in GetBlocks(blocks, region))
            {
                if(b.IsEmpty())
                    continue;

                if (ids.Contains(b.ID))
                    result.Add(b);
            }

            return result;
        }

        public static Tuple<int, int> GetDimensions(Blocks blocks)
        {
            var fallback = new Tuple<int, int>(0, 0);
            var minY = int.MaxValue;
            var minX = int.MaxValue;
            var maxX = int.MinValue;
            var maxY = int.MinValue;
            var any = false;

            if (blocks == null)
                return fallback;

            foreach (var b in blocks.GetBlocks(true))
            {
                if (b.IsEmpty())
                    continue;

                any = true;
                var x = b.Position.Value.X;
                var y = b.Position.Value.Y;

                if (x < minX)
                    minX = x;

                if (y < minY)
                    minY = y;

                if (x > maxX)
                    maxX = x;

                if (y > maxY)
                    maxY = y;
            }

            if (!any)
                return fallback;

            return new Tuple<int, int>(maxX - minX + 1, maxY - minY + 1);
        }

        public static List<SimpleBlock> GetFloodFill(Blocks blocks, MyPoint? startPoint, int id, MyRegion region)
        {
            var result = new List<SimpleBlock>();
            var stack = new Stack<MyPoint>();

            if (blocks == null || startPoint == null)
                return result;

            stack.Push(startPoint.Value);
            var lowerLimit = new MyPoint(0, 0);
            var upperLimit = new MyPoint(Blocks.SIZE, Blocks.SIZE);
            var visited = new List<MyPoint>();
            var startBlock = blocks.GetBlock(startPoint);
            var blockLimit = 5001;
            var maxBlocks = Math.Min(Blocks.LIMIT - blocks.BlockCount, blockLimit);
            var shownWarning = false;

            if (region.IsComplete())
            {
                lowerLimit = region.Start.Value;
                upperLimit = region.End.Value;
            }

            while (stack.Count > 0)
            {
                var point = stack.Pop();

                if (!shownWarning && result.Count >= 1000)
                {
                    shownWarning = true;
                    var r = UserQuestionWindow.ShowWarning("Over 1000 blocks has been added."
                        + Environment.NewLine + Environment.NewLine
                        + "Do you wish to continue?", false);

                    if (r != UserQuestionWindow.QuestionResult.Yes)
                        return new List<SimpleBlock>();
                }

                if (maxBlocks <= result.Count)
                {
                    if(maxBlocks < blockLimit)
                        throw new BlockLimitException();

                    throw new Exception("Too many blocks...only use the flood-fill tool in a closed region."
                    + Environment.NewLine + Environment.NewLine
                    + "Operation has been cancelled.");
                }


                if (visited.Contains(point))
                    continue;

                visited.Add(point);

                if (point.X < lowerLimit.X || point.X >= upperLimit.X)
                    continue;

                if (point.Y < lowerLimit.Y || point.Y >= upperLimit.Y)
                    continue;

                var currentBlock = blocks.GetBlock(point.X, point.Y);

                if (currentBlock.ID != startBlock.ID)
                    continue;

                result.Add(new SimpleBlock(id, point));
                stack.Push(new MyPoint(point.X - 1, point.Y));
                stack.Push(new MyPoint(point.X + 1, point.Y));
                stack.Push(new MyPoint(point.X, point.Y - 1));
                stack.Push(new MyPoint(point.X, point.Y + 1));
            }

            return result;
        }

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

                if (posX < 0 || posY < 0 || posX > Blocks.SIZE || posY > Blocks.SIZE)
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

                var x = b.Position.Value.X - previousX;
                var y = b.Position.Value.Y - previousY;
                previousX = b.Position.Value.X;
                previousY = b.Position.Value.Y;

                var block = new Block(x, y, b.ID, b.Options);
                pr2Blocks.Add(block);
            }

            return pr2Blocks;
        }

        public static void Move(Game game, List<SimpleBlock> blocks, int x, int y)
        {
            if(game == null || blocks == null)
                return;

            var result = new List<SimpleBlock>();

            foreach (var b in blocks)
            {
                if (b.IsEmpty())
                    continue;

                var point = new MyPoint(b.Position.Value.X + x, b.Position.Value.Y + y);
                var block = new SimpleBlock(b.ID, point, b.Options);

                result.Add(block);
            }


            game.RemoveBlocks(blocks);
            game.AddBlocks(result);
        }

        internal static IEnumerable<SimpleBlock> MoveSelection(List<SimpleBlock> blocks, MyPoint? index)
        {
            var result = new List<SimpleBlock>();

            if (blocks == null || !blocks.Any() || index == null)
                return result;

            var width  = ArrayUtil.GetMaxWidth(blocks);
            var height = ArrayUtil.GetMaxHeight(blocks);

            return blocks
                .RemoveEmpty()
                .Select(b => b.Move(index.Value.X + b.Position.Value.X - width, index.Value.Y + b.Position.Value.Y - height))
                .ToList();
        }
    }
}
