using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockEditor.Models
{

    public class UserSelection
    {

        public MyRegion MapRegion { get; }
        public MyRegion ImageRegion { get; }

        public UserSelection()
        {
            MapRegion = new MyRegion();
            ImageRegion = new MyRegion();
        }

        public void Reset()
        {
            MapRegion.Reset();
            ImageRegion.Reset();
        }

        public bool HasSelectedRegion => MapRegion.IsComplete() && ImageRegion.IsComplete();

        private List<SimpleBlock> GetSelection(Map map)
        {

            if (map == null || MapRegion == null || !MapRegion.IsComplete())
                return null;

            var start  = MapRegion.Start.Value;
            var end    = MapRegion.End.Value;
            var result = new List<SimpleBlock>();


            for (int y = start.Y; y < end.Y; y++)
            {
                for (int x = start.X; x < end.X; x++)
                {
                    var normalBlock = map.Blocks.GetBlock(x, y, false);
                    var startBlock  = map.Blocks.StartBlocks.GetBlock(x, y);

                    if(!normalBlock.IsEmpty())
                        result.Add(normalBlock.Move(x - start.X, y - start.Y));

                    if (!startBlock.IsEmpty())
                        result.Add(startBlock.Move(x - start.X, y - start.Y));
                }
            }

            return result;
        }

        public void OnMouseDown(MyPoint? image, MyPoint? map)
        {
            Reset();
            MapRegion.Point1   = map;
            ImageRegion.Point1 = image;
        }

        public void OnMouseUp(MyPoint? image, MyPoint? map)
        {
            MapRegion.Point2 = map;
            ImageRegion.Point2 = image;
        }

        public void CreateSelection(Map map)
        {
            var selection = GetSelection(map);
            BlockSelection.OnNewSelection(selection);
        }

        public bool SelectedRegionContainsBlocks(Map map)
        {
            if(map == null)
                return false;

            var selection = GetSelection(map);

            if(selection == null)
                return false;

            return selection.Any(b => !b.IsEmpty());
        }


    }
}
