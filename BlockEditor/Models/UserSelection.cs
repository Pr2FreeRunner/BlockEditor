using System;

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

        private int?[,] GetSelection(Map map)
        {
            if (map == null || MapRegion == null || !MapRegion.IsComplete())
                return null;

            var start = MapRegion.Start.Value;
            var end   = MapRegion.End.Value;

            var selection = new int?[end.X - start.X, end.Y - start.Y];

            for (int y = start.Y; y < end.Y; y++)
            {
                for (int x = start.X; x < end.X; x++)
                {
                    var b = map.Blocks.GetBlock(x, y);

                    if(b.IsEmpty())
                        continue;

                    selection[x - start.X, y - start.Y] = b.ID;
                }
            }

            return selection;
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


    }
}
