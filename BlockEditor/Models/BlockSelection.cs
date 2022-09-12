﻿using BlockEditor.Helpers;
using BlockEditor.Utils;
using System;

namespace BlockEditor.Models
{

    public static class BlockSelection
    {

        public static RelayCommand RotateCommand { get; }
        public static RelayCommand VerticalFlipCommand { get; }
        public static RelayCommand HorizontalFlipCommand { get; }



        private static readonly object _lock = new object();
        private static SimpleBlock[,] _selectedBlocks;
        public static SimpleBlock[,] SelectedBlocks
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
                    PreviouselectedBlocks = _selectedBlocks;
                    _selectedBlocks = temp;
                }
            }
        }

        private static SimpleBlock[,] _previousSelectedBlocks;
        public static SimpleBlock[,] PreviouselectedBlocks
        {
            get
            {
                return _previousSelectedBlocks;
            }
            set
            {
                if(value == null)
                    return;
                _previousSelectedBlocks = value;
            }
        }

        private static int? _selectedBlock;
        public static int? SelectedBlock 
        { 
            get { return _selectedBlock; }
            set { _selectedBlock = value; }
        }

        public static string SelectedBlockOption { get; set; }

        public static Action CleanUserBlockControl { get; set; }

        static BlockSelection()
        {
            RotateCommand = new RelayCommand((_) => Rotate(), (_) => CanRotate());
            VerticalFlipCommand = new RelayCommand((_) => VerticalFlip(), (_) => CanRotate());
            HorizontalFlipCommand = new RelayCommand((_) => HorizontalFlip(), (_) => CanRotate());

        }

        public static void OnNewSelection(SimpleBlock[,] selection)
        {
            SelectedBlocks = selection;
        }

        private static bool CanRotate()
        {
            return SelectedBlocks != null;
        }

        public static void Reset()
        {
            SelectedBlocks = null;
            SelectedBlock = null;
            SelectedBlockOption = null;
            CleanUserBlockControl?.Invoke();
        }

        public static void Rotate()
        {
            if (!CanRotate())
                return;

            SelectedBlocks = ArrayUtil.RotateRight(SelectedBlocks);
        }

        public static void VerticalFlip()
        {
            if (!CanRotate())
                return;

            SelectedBlocks = ArrayUtil.VerticalFlip(SelectedBlocks);
        }

        public static void HorizontalFlip()
        {
            if (!CanRotate())
                return;

            SelectedBlocks = ArrayUtil.HorizontalFlip(SelectedBlocks);
        }

        internal static void ActivatePreviousSelection()
        {
            if(SelectedBlocks != null)
                return;

            Reset();
            SelectedBlocks = PreviouselectedBlocks;
        }
    }
}
