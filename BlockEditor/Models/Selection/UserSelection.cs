using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockEditor.Models
{

    public class UserSelection
    {

        private readonly Func<MyPoint?, MyPoint?> _getMapIndex;

        public MyRegion ImageRegion { get; }
        public MyRegion MapRegion => CreateMapIndex();

        public UserSelection(Func<MyPoint?, MyPoint?> getMapIndex)
        {
            _getMapIndex = getMapIndex;
            ImageRegion = new MyRegion();
        }

        public void Reset()
        {
            ImageRegion.Reset();
        }

        public bool HasSelectedRegion => ImageRegion.IsComplete();

        private MyRegion CreateMapIndex()
        {
            var point1 = _getMapIndex?.Invoke(ImageRegion.Point1);
            var point2 = _getMapIndex?.Invoke(ImageRegion.Point2);

            return new MyRegion() { Point1 = point1, Point2 = point2 };
        }

        private List<SimpleBlock> GetSelection(Map map)
        {
            var region = MapRegion;

            if (map == null || !region.IsComplete())
                return null;

            var start = region.Start.Value;
            var end = region.End.Value;
            var result = new List<SimpleBlock>();


            for (int y = start.Y; y < end.Y; y++)
            {
                for (int x = start.X; x < end.X; x++)
                {
                    var normalBlock = map.Blocks.GetBlock(x, y, false);
                    var startBlocks = map.Blocks.StartBlocks.GetBlocks(x, y);

                    if (!normalBlock.IsEmpty())
                        result.Add(normalBlock.Move(x - start.X, y - start.Y));

                    if (startBlocks != null)
                        result.AddRange(startBlocks.RemoveEmpty().Select(b => b.Move(x - start.X, y - start.Y)));
                }
            }

            return result;
        }

        public void OnMouseDown(MyPoint? image)
        {
            Reset();
            ImageRegion.Point1 = image;
        }

        public void OnMouseUp(MyPoint? image)
        {
            ImageRegion.Point2 = image;
        }

        public void CreateSelection(Map map)
        {
            BlockSelection.OnNewSelection(GetSelection(map));
        }

        public bool SelectedRegionContainsBlocks(Map map)
        {
            if (map == null)
                return false;

            var selection = GetSelection(map);

            return selection.AnyBlocks();
        }


    }
}
