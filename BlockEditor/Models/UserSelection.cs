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

        private SimpleBlock[,] GetSelection(Map map)
        {
            if (map == null || MapRegion == null || !MapRegion.IsComplete())
                return null;

            var start = MapRegion.Start.Value;
            var end   = MapRegion.End.Value;

            var selection = new SimpleBlock[end.X - start.X, end.Y - start.Y];

            for (int y = start.Y; y < end.Y; y++)
            {
                for (int x = start.X; x < end.X; x++)
                {
                    var b = map.Blocks.GetBlock(x, y);

                    if(b.IsEmpty())
                        continue;

                    var xPos = x - start.X;
                    var yPos = y - start.Y;

                    selection[xPos, yPos] = b.Move(xPos, yPos);
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

        public bool SelectedRegionContainsBlocks(Map map)
        {
            if(map == null)
                return false;

            var selection = GetSelection(map);

            if(selection == null)
                return false;

            var length0 = selection.GetLength(0);
            var length1 = selection.GetLength(1);

            for (int i = 0; i < length0; i++)
            {
                for (int j = 0; j < length1; j++)
                {
                    if (!selection[i, j].IsEmpty())
                        return true;
                }
            }

            return false;
        }


    }
}
