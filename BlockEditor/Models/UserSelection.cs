using System;

namespace BlockEditor.Models
{
    public class UserSelection
    {

        private MyPoint? _mouseDownMapIndex { get; set; }
        private MyPoint? _mouseUpMapIndex { get; set; }

        private MyPoint? _mouseDownImageIndex { get; set; }
        private MyPoint? _mouseUpImageIndex { get;  set; }

        public event Action<int?[,]> OnNewSelection;


        public MyPoint? StartMapIndex
        {
            get
            {
                if(_mouseDownMapIndex == null)
                    return null;
                
                if(_mouseUpMapIndex == null)
                    return _mouseDownMapIndex;

                var x = Math.Min(_mouseDownMapIndex.Value.X, _mouseUpMapIndex.Value.X);
                var y = Math.Min(_mouseDownMapIndex.Value.Y, _mouseUpMapIndex.Value.Y);

                return new MyPoint(x, y);
            }
        }

        public MyPoint? EndMapIndex
        {
            get
            {
                if (_mouseDownMapIndex == null || _mouseUpMapIndex == null)
                    return null;

                var x = Math.Max(_mouseDownMapIndex.Value.X, _mouseUpMapIndex.Value.X);
                var y = Math.Max(_mouseDownMapIndex.Value.Y, _mouseUpMapIndex.Value.Y);

                return new MyPoint(x, y);
            }
        }


        public MyPoint? StartImageIndex
        {
            get
            {
                if (_mouseDownImageIndex == null)
                    return null;

                if (_mouseUpImageIndex == null)
                    return _mouseDownImageIndex;

                var x = Math.Min(_mouseDownImageIndex.Value.X, _mouseUpImageIndex.Value.X);
                var y = Math.Min(_mouseDownImageIndex.Value.Y, _mouseUpImageIndex.Value.Y);

                return new MyPoint(x, y);
            }
        }
        public MyPoint? EndImageIndex
        {
            get
            {
                if (_mouseDownImageIndex == null || _mouseUpImageIndex == null)
                    return null;

                var x = Math.Max(_mouseDownImageIndex.Value.X, _mouseUpImageIndex.Value.X);
                var y = Math.Max(_mouseDownImageIndex.Value.Y, _mouseUpImageIndex.Value.Y);

                return new MyPoint(x, y);
            }
        }

        public void Reset()
        {
            _mouseDownMapIndex = null;
            _mouseUpMapIndex = null;

            _mouseDownImageIndex = null;
            _mouseUpImageIndex = null;
        }

        private int?[,] GetSelection(Map map)
        {
            if (map == null || _mouseDownMapIndex == null || _mouseUpMapIndex == null)
                return null;

            var start = StartMapIndex;
            var end   = EndMapIndex;

            if(start == null || end == null)
                return null;

            var selection = new int?[end.Value.X - start.Value.X + 1, end.Value.Y - start.Value.Y + 1];

            for (int y = start.Value.Y; y <= end.Value.Y; y++)
            {
                for (int x = start.Value.X; x <= end.Value.X; x++)
                {
                    selection[x - start.Value.X, y - start.Value.Y] = map.Blocks.GetBlockId(x, y);
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
            _mouseDownImageIndex = image;
            _mouseDownMapIndex = map;

            _mouseUpImageIndex = null;
            _mouseUpMapIndex = null;
        }

        public void OnMouseUp(MyPoint? image, MyPoint? map)
        {
            _mouseUpImageIndex = image;
            _mouseUpMapIndex = map;
        }

        public void OnKeydown(Map map)
        {
            var selection = GetSelection(map);
            OnNewSelection?.Invoke(selection);
            Reset();
        }
    }
}
