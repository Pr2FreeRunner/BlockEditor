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
        public RelayCommand SelectCommand { get; }
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

        public BlockSelection()
        {
            RotateRightCommand = new RelayCommand((_) => RotateRight(), (_) => CanRotate());
            RotateRightCommand = new RelayCommand((_) => RotateLeft(), (_) => CanRotate());
            SelectCommand      = new RelayCommand(SelectCommandExecute);
            UserSelection      = new UserSelection();

            UserSelection.OnNewSelection += OnUserSelection;
        }

        private void SelectCommandExecute(object obj)
        {
            SelectedBlock = null;
            UserSelection.OnSelectionClick();
            OnSelectionClick?.Invoke();
        }

        private void OnUserSelection(int?[,] selection)
        {
            SelectedBlocks = selection;
        }

        private bool CanRotate()
        {
            return SelectedBlocks != null;
        }

        public void Clean()
        {
            UserSelection.Reset();
            SelectedBlock = null;
            SelectedBlocks = null;
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
