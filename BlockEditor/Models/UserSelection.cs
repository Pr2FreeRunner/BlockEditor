using System;

namespace BlockEditor.Models
{
    public class UserSelection
    {
        public bool Active { get; set; }

        public MyPoint? StartIndex { get; private set; }
        public MyPoint? EndIndex { get; private set; }
        private event Action<int?[,]> OnNewSelection;


        public UserSelection(Action<int?[,]> onNewSelectin)
        {
            OnNewSelection = onNewSelectin;
        }

        private void Reset()
        {
            Active = false;
            StartIndex = null;
            EndIndex = null;
        }

        private int?[,] GetSelection(Map map, bool delete)
        {
            if (map == null || StartIndex == null || EndIndex == null)
                return null;

            var startRow = Math.Min(StartIndex.Value.X, EndIndex.Value.X);
            var startColumn = Math.Min(StartIndex.Value.Y, EndIndex.Value.Y);
            var endRow = Math.Max(StartIndex.Value.X, EndIndex.Value.X);
            var endColumn = Math.Max(StartIndex.Value.Y, EndIndex.Value.Y);

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

        public void OnMouseDown(MyPoint p)
        {
            if (Active)
            {
                Reset();
                return;
            }

            StartIndex = p;
            EndIndex = null;
            Active = true;
        }

        public void OnMouseUp(MyPoint p)
        {
            if (!Active)
            {
                Reset();
                return;
            }

            EndIndex = p;
        }

        public void OnKeydown(Map map)
        {
            if (!Active)
            {
                Reset();
                return;
            }

            var selection = GetSelection(map, false);
            OnNewSelection?.Invoke(selection);
            Reset();
        }
    }
}
