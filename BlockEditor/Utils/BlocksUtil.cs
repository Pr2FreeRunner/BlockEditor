using BlockEditor.Models;
using BlockEditor.Views.Windows;
using System;
using System.Collections.Generic;

namespace BlockEditor.Helpers
{
    public static class BlocksUtil
    {
        public static List<SimpleBlock> ReplaceBlock(Blocks blocks, List<int> replace, List<int> add, MyRegion region)
        {
            var result = new List<SimpleBlock>();

            if (blocks == null || replace == null || add == null || replace.Count != add.Count)
                return result;

            var lowerLimit = new MyPoint(0, 0);
            var upperLimit = new MyPoint(Blocks.SIZE, Blocks.SIZE);
            var notFound = -1;

            if (region != null && region.IsComplete())
            {
                lowerLimit = region.Start.Value;
                upperLimit = region.End.Value;
            }

            for (int x = lowerLimit.X; x < upperLimit.X; x++)
            {
                for (int y = lowerLimit.Y; y < upperLimit.Y; y++)
                {
                    var b = blocks.GetBlock(x, y, false);
                    var index = replace.IndexOf(b.ID);

                    if (index != notFound)
                        result.Add(new SimpleBlock(add[index], x, y));
                }
            }

            return result;
        }

        public static List<SimpleBlock> GetBlocks(Blocks blocks, MyRegion region, bool startBlocks = true)
        {
            var result = new List<SimpleBlock>();

            if (blocks == null)
                return result;

            var lowerLimit = new MyPoint(0, 0);
            var upperLimit = new MyPoint(Blocks.SIZE, Blocks.SIZE);

            if (region != null && region.IsComplete())
            {
                lowerLimit = region.Start.Value;
                upperLimit = region.End.Value;
            }

            for (int x = lowerLimit.X; x < upperLimit.X; x++)
                for (int y = lowerLimit.Y; y < upperLimit.Y; y++)
                    result.Add(blocks.GetBlock(x, y, startBlocks));

            return result;
        }

        public static List<SimpleBlock> RemoveBlocks(Blocks blocks, List<int> ids, MyRegion region)
        {
            var result = new List<SimpleBlock>();

            if (blocks == null || ids == null)
                return result;

            var lowerLimit = new MyPoint(0, 0);
            var upperLimit = new MyPoint(Blocks.SIZE, Blocks.SIZE);

            if (region != null && region.IsComplete())
            {
                lowerLimit = region.Start.Value;
                upperLimit = region.End.Value;
            }

            for (int x = lowerLimit.X; x < upperLimit.X; x++)
            {
                for (int y = lowerLimit.Y; y < upperLimit.Y; y++)
                {
                    var b = blocks.GetBlock(x, y);

                    if (ids.Contains(b.ID))
                        result.Add(b);
                }
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
            var maxBlocks = Math.Min(Blocks.LIMIT - blocks.BlockCount, 5_001);
            var shownWarning = false;

            if (region != null && region.IsComplete())
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
                    throw new Exception("Too many blocks...only use the flood-fill tool in a closed region."
                        + Environment.NewLine + Environment.NewLine
                        + "Operation has been cancelled.");

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

    }
}
