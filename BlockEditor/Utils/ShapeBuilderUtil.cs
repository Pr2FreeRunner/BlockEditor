using BlockEditor.Models;
using BlockEditor.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockEditor.Utils
{
    public static class ShapeBuilderUtil
    {
        public enum ShapeType { Rectangle, Square, Circle, Ellipse }
        public static ShapeType Type { get; set; }
        public static bool Fill { get; set; }
        public static int Probablity { get; set; }

        public static List<SimpleBlock> Build(Map map, int id, MyRegion region)
        {
            var fallback = new List<SimpleBlock>();

            if (map == null || !region.IsComplete())
                return fallback;

            switch (Type)
            {
                case ShapeType.Rectangle:
                    if(Fill)
                        return GetRectangleFill(map, id, region);
                    else
                        return GetRectangle(map, id, region);

                case ShapeType.Square:
                    return GetSquare(map, id, region);

                case ShapeType.Circle:
                    return GetCircle(map, id, region);

                case ShapeType.Ellipse:
                    return GetEllipse(map, id, region);

                default: return fallback;
            }
        }

        public static bool PickShape()
        {
            var window  = new PickShapeWindow(Type);
            var success = window.ShowDialog();

            if(success != true)
                return false;

            Type = window.Result;
            Fill = window.Fill;
            Probablity = window.Probability;

            return true;
        }


        private static MyRegion GetSquare(MyRegion region)
        {
            var width = region.Width;
            var height = region.Height;
            var size   = Math.Min(width.Value, height.Value);

            return new MyRegion
            {
                Point1 = region.Start,
                Point2 = new MyPoint(region.Start.Value.X + size - 1, region.Start.Value.Y + size - 1)
            };
        }

        private static List<SimpleBlock> GetCircle(Map map, int id, MyRegion region)
        {
            var square = GetSquare(region);

            return GetEllipse(map, id, square);
        }

        private static List<SimpleBlock> GetSquare(Map map, int id, MyRegion region)
        {
            var square = GetSquare(region); 

            if(Fill)
                return GetRectangleFill(map, id, square);
            else
                return GetRectangle(map, id, square);
        }

        private static List<SimpleBlock> GetRectangleFill(Map map, int id, MyRegion region)
        {
            var result = new List<SimpleBlock>();

            for (int x = region.Start.Value.X; x < region.End.Value.X; x++)
                for (int y = region.Start.Value.Y; y < region.End.Value.Y; y++)
                    AddBlock(map, result, id, x, y);

            return result;
        }

        private static void AddBlock(Map map, List<SimpleBlock> result, int id, int x, int y)
        {
            if(Probablity < 100)
            {
                if(!(RandomUtil.GetRandom(1, 99) < Probablity))
                    return;
            }

            if (map.Blocks.Overwrite)
            {
                result.Add(new SimpleBlock(id, x, y));
            }
            else
            {
                var currentBlock = map.Blocks.GetBlock(x, y);

                if (currentBlock.IsEmpty())
                    result.Add(new SimpleBlock(id, x, y));
            }
        }

        private static List<SimpleBlock> GetRectangle(Map map, int id, MyRegion region)
        {
            var result = new List<SimpleBlock>();
            var outlineSize = 2;

            for (int x = region.Start.Value.X; x < region.End.Value.X; x++)
                for (int y = region.Start.Value.Y; y < region.End.Value.Y && y < region.Start.Value.Y + outlineSize; y++)
                    AddBlock(map, result, id, x, y);

            for (int x = region.Start.Value.X; x < region.End.Value.X; x++)
                for (int y = region.End.Value.Y - 1; y > region.Start.Value.Y && y > region.End.Value.Y - outlineSize - 1; y--)
                    AddBlock(map, result, id, x, y);

            for (int x = region.Start.Value.X; x < region.End.Value.X && x < region.Start.Value.X + outlineSize; x++)
                for (int y = region.Start.Value.Y; y < region.End.Value.Y; y++)
                    AddBlock(map, result, id, x, y);

            for (int x = region.End.Value.X - 1; x > region.Start.Value.X && x > region.End.Value.X - outlineSize - 1; x--)
                for (int y = region.Start.Value.Y; y < region.End.Value.Y; y++)
                    AddBlock(map, result, id, x, y);

            return result.GroupBy(x => x.Position).Select(x => x.First()).ToList();
        }

        private static List<SimpleBlock> GetEllipse(Map map, int id, MyRegion region)
        {
            var result = new List<SimpleBlock>();
            var minSize = 10;

            if(region.Width < minSize || region.Height < minSize)
                throw new Exception("The region size is too small.");

            double a   = region.Width.Value / 2.0;
            double b   = region.Height.Value / 2.0;
            var margin = 0.1 * ((75.0 + region.Height.Value + region.Width.Value) / (region.Height.Value + region.Width.Value));

            for (double y = -b; y < b; y++)
            {
                for (double x = -a; x < a; x++)
                {
                    var distance    = (x / a) * (x / a) + (y / b) * (y / b);
                    var indexWidth  = (int)(x + a) + region.Start.Value.X;
                    var indexHeight = (int)(y + b) + region.Start.Value.Y;

                    if (distance < 1.01 && (Fill || distance > 1.0 - margin))
                        AddBlock(map, result, id, indexWidth, indexHeight);
                }
            }

            return result;
        }

    }
}
