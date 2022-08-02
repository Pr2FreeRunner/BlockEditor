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

        private int?[,] GetSelection(Map map, bool delete)
        {
            if (map == null || StartMapIndex == null || EndMapIndex == null)
                return null;

            var startRow    = Math.Min(StartMapIndex.Value.X, EndMapIndex.Value.X);
            var startColumn = Math.Min(StartMapIndex.Value.Y, EndMapIndex.Value.Y);
            var endRow      = Math.Max(StartMapIndex.Value.X, EndMapIndex.Value.X);
            var endColumn   = Math.Max(StartMapIndex.Value.Y, EndMapIndex.Value.Y);

            var selection = new int?[endRow - startRow, endColumn - startColumn];

            for (int row = startRow; row < endRow; row++)
            {
                for (int column = startColumn; column < endColumn; column++)
                {
                    var id = map.Blocks.GetBlockId(row, column);
                    selection[row - startRow, column - endColumn] = id;

                    if (id != null && delete)
                        map.Blocks.Delete(new MyPoint(row, column));
                }
            }

            Reset();
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
            var selection = GetSelection(map, false);
            OnNewSelection?.Invoke(selection);
            Reset();
        }
    }
}
