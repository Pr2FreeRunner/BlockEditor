using System;

namespace BlockEditor.Models
{
    public class UserSelection
    {

        private MyPoint? _keyDownMapIndex { get; set; }
        private MyPoint? _keyUpMapIndex { get; set; }

        private MyPoint? _keyDownImageIndex { get; set; }
        private MyPoint? _keyUpImageIndex { get;  set; }

        public event Action<int?[,]> OnNewSelection;

        public MyPoint? StartMapIndex
        {
            get
            {
                if(_keyDownMapIndex == null || _keyUpMapIndex == null)
                    return null;

                var x = Math.Min(_keyDownMapIndex.Value.X, _keyUpMapIndex.Value.X);
                var y = Math.Min(_keyDownMapIndex.Value.Y, _keyUpMapIndex.Value.Y);

                return new MyPoint(x, y);
            }
        }

        public MyPoint? EndMapIndex
        {
            get
            {
                if (_keyDownMapIndex == null || _keyUpMapIndex == null)
                    return null;

                var x = Math.Max(_keyDownMapIndex.Value.X, _keyUpMapIndex.Value.X);
                var y = Math.Max(_keyDownMapIndex.Value.Y, _keyUpMapIndex.Value.Y);

                return new MyPoint(x, y);
            }
        }


        public MyPoint? StartImageIndex
        {
            get
            {
                if (_keyDownImageIndex == null || _keyUpImageIndex == null)
                    return null;

                var x = Math.Min(_keyDownImageIndex.Value.X, _keyUpImageIndex.Value.X);
                var y = Math.Min(_keyDownImageIndex.Value.Y, _keyUpImageIndex.Value.Y);

                return new MyPoint(x, y);
            }
        }
        public MyPoint? EndImageIndex
        {
            get
            {
                if (_keyDownImageIndex == null || _keyUpImageIndex == null)
                    return null;

                var x = Math.Max(_keyDownImageIndex.Value.X, _keyUpImageIndex.Value.X);
                var y = Math.Max(_keyDownImageIndex.Value.Y, _keyUpImageIndex.Value.Y);

                return new MyPoint(x, y);
            }
        }

        public void Reset()
        {
            _keyDownMapIndex = null;
            _keyUpMapIndex = null;

            _keyDownImageIndex = null;
            _keyUpImageIndex = null;
        }

        private int?[,] GetSelection(Map map)
        {
            if (map == null || _keyDownMapIndex == null || _keyUpMapIndex == null)
                return null;

            var start = StartMapIndex;
            var end   = EndMapIndex;

            if(start == null || end == null)
                return null;

            var selection = new int?[end.Value.X - start.Value.X, end.Value.Y - start.Value.Y];

            for (int y = start.Value.Y; y < end.Value.Y; y++)
            {
                for (int x = start.Value.X; x < end.Value.X; x++)
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
            _keyDownImageIndex = image;
            _keyDownMapIndex = map;

            _keyUpImageIndex = null;
            _keyUpMapIndex = null;
        }

        public void OnMouseUp(MyPoint? image, MyPoint? map)
        {
            _keyUpImageIndex = image;
            _keyUpMapIndex = map;
        }

        public void OnKeydown(Map map)
        {
            var selection = GetSelection(map);
            OnNewSelection?.Invoke(selection);
            Reset();
        }
    }
}
