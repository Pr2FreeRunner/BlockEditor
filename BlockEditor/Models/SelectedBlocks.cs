using BlockEditor.Helpers;
using BlockEditor.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockEditor.Models
{

    public class SelectedBlocks
    {

        public RelayCommand RotateRightCommand { get; }
        public RelayCommand RotateLeftCommand { get; }


        private int?[,] _selection;
        private readonly object _lock = new object();
        public int?[,] Selection
        {
            get
            {
                return _selection;
            }
            set
            {
                var temp = ArrayUtil.MinimizeSize(value);

                lock (_lock)
                {
                    _selection = temp;
                }
            }
        }

        private void Dummy()
        {
            var size = 4;
            var selection = new int?[size + 2, size];

            selection[0, 0] = 1;
            selection[0, 1] = 1;
            selection[0, 2] = 1;
            selection[0, 3] = 1;
            selection[1, 3] = 1;
            selection[2, 3] = 1;


            Selection = selection;
        }

        public SelectedBlocks()
        {
            RotateRightCommand = new RelayCommand((_) => RotateRight(), (_) => CanRotate());
            RotateRightCommand = new RelayCommand((_) => RotateLeft(), (_) => CanRotate());

            Dummy();
        }

        private bool CanRotate()
        {
            return Selection != null;
        }

        public void RotateLeft()
        {
            if (!CanRotate())
                return;

            var temp = Selection;
            var rotations = 3;

            for (int i = 0; i < rotations; i++)
                temp = ArrayUtil.RotateRight(temp);

            Selection = temp;
        }

        public void RotateRight()
        {
            if (!CanRotate())
                return;

            Selection = ArrayUtil.RotateRight(Selection);
        }

    }
}
