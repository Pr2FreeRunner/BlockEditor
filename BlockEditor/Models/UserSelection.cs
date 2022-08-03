using System;

namespace BlockEditor.Models
{
    public class UserSelection
    {

        public MyPoint? StartMapIndex { get; private set; }
        public MyPoint? EndMapIndex { get; private set; }

        public MyPoint? StartImageIndex { get; private set; }
        public MyPoint? EndImageIndex { get; private set; }

        public event Action<int?[,]> OnNewSelection;


        public void Reset()
        {
            StartMapIndex = null;
            EndMapIndex = null;

            StartImageIndex = null;
            EndImageIndex = null;
        }

        private int?[,] GetSelection(Map map)
        {
            if (map == null || StartMapIndex == null || EndMapIndex == null)
                return null;

            var startY = Math.Min(StartMapIndex.Value.Y, EndMapIndex.Value.Y);
            var startX = Math.Min(StartMapIndex.Value.X, EndMapIndex.Value.X);
            var endY   = Math.Max(StartMapIndex.Value.Y, EndMapIndex.Value.Y) + 1;
            var endX   = Math.Max(StartMapIndex.Value.X, EndMapIndex.Value.X) + 1;

            var selection = new int?[endX - startX, endY - startY];

            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX ; x++)
                {
                    selection[x - startX, y - startY] = map.Blocks.GetBlockId(x, y);
                }
            }

            return selection;
        }

        public void OnSelectionClick()
        {
            Reset();
        }

        public void OnMouseDown(MyPoint? image, MyPoint? map)
        {
            StartImageIndex = image;
            StartMapIndex = map;

            EndImageIndex = null;
            EndMapIndex = null;
        }

        public void OnMouseUp(MyPoint? image, MyPoint? map)
        {
            EndImageIndex = image;
            EndMapIndex = map;
        }

        public void OnKeydown(Map map)
        {
            var selection = GetSelection(map);
            OnNewSelection?.Invoke(selection);
            Reset();
        }
    }
}
