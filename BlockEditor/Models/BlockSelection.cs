using BlockEditor.Helpers;
using BlockEditor.Utils;
using System;

namespace BlockEditor.Models
{

    public static class BlockSelection
    {

        public static RelayCommand RotateCommand { get; }

        private static readonly object _lock = new object();
        private static int?[,] _selectedBlocks;
        public static int?[,] SelectedBlocks
        {
            get
            {
                return _selectedBlocks;
            }
            set
            {
                var temp = ArrayUtil.MinimizeSize(value);

                lock (_lock)
                {
                    _selectedBlocks = temp;
                }
            }
        }

        private static int? _selectedBlock;
        public static int? SelectedBlock 
        { 
            get { return _selectedBlock; }
            set { _selectedBlock = value; }
        }

        static BlockSelection()
        {
            RotateCommand = new RelayCommand((_) => Rotate(), (_) => CanRotate());
        }

        public static void OnNewSelection(int?[,] selection)
        {
            SelectedBlocks = selection;
        }

        private static bool CanRotate()
        {
            return SelectedBlocks != null;
        }

        public static void Reset(bool userSelection = true)
        {
            SelectedBlocks = null;
            SelectedBlock = null;
        }

        public static void Rotate()
        {
            if (!CanRotate())
                return;

            SelectedBlocks = ArrayUtil.RotateRight(SelectedBlocks);
        }

  
    }
}
