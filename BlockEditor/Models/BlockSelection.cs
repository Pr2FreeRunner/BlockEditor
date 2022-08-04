using BlockEditor.Helpers;
using BlockEditor.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockEditor.Models
{

    public class BlockSelection
    {

        public RelayCommand RotateRightCommand { get; }
        public RelayCommand RotateLeftCommand { get; }
        public UserSelection UserSelection { get; }

        public event Action OnSelectionClick;

        private int?[,] _selectedBlocks;
        private readonly object _lock = new object();
        public int?[,] SelectedBlocks
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

        private int? _selectedBlock;
        public int? SelectedBlock 
        { 
            get { return _selectedBlock; }
            set { _selectedBlock = value; }
        }

        private Action _cleanBlockSelection { get; }

        public BlockSelection(Action cleanBlockSelection)
        {
            RotateRightCommand    = new RelayCommand((_) => RotateLeft(), (_) => CanRotate());
            RotateLeftCommand     = new RelayCommand((_) => RotateRight(),  (_) => CanRotate());
            UserSelection         = new UserSelection();
            _cleanBlockSelection  = cleanBlockSelection;

            UserSelection.OnNewSelection += OnUserSelection;
        }

        private void OnUserSelection(int?[,] selection)
        {
            SelectedBlocks = selection;
        }

        private bool CanRotate()
        {
            return SelectedBlocks != null;
        }

        public void Reset(bool cleanSelectedBlock = true)
        {
            _cleanBlockSelection?.Invoke();
            UserSelection.Reset();
            SelectedBlocks = null;

            if(cleanSelectedBlock)
                SelectedBlock = null;
        }

        public void RotateLeft()
        {
            if (!CanRotate())
                return;

            var temp = SelectedBlocks;
            var rotations = 3;

            for (int i = 0; i < rotations; i++)
                temp = ArrayUtil.RotateRight(temp);

            SelectedBlocks = temp;
        }

        public void RotateRight()
        {
            if (!CanRotate())
                return;

            SelectedBlocks = ArrayUtil.RotateRight(SelectedBlocks);
        }

  
    }
}
