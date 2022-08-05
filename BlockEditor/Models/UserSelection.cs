using System;

namespace BlockEditor.Models
{

    public class UserSelection
    {

        public MyRegion MapRegion { get; }
        public MyRegion ImageRegion { get; }

        public event Action<int?[,]> OnNewSelection;


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
                    selection[x - start.X, y - start.Y] = map.Blocks.GetBlockId(x, y);
                }
            }

            return selection;
        }

        public void OnMouseDown(MyPoint? image, MyPoint? map)
        {
            MapRegion.Point1   = map;
            ImageRegion.Point1 = image;

            MapRegion.Point2   = null;
            ImageRegion.Point2 = null;
        }

        public void OnMouseUp(MyPoint? image, MyPoint? map)
        {
            MapRegion.Point2 = map;
            ImageRegion.Point2 = image;
        }

        public void CreateSelection(Map map)
        {
            var selection = GetSelection(map);
            OnNewSelection?.Invoke(selection);
            Reset();
        }


    }
}
