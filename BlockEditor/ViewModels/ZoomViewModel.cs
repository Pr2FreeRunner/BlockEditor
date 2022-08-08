using BlockEditor.Helpers;
using BlockEditor.Models;
using BlockEditor.Utils;
using System;
using System.Globalization;
using static BlockEditor.Models.BlockImages;

namespace BlockEditor.ViewModels
{
    public class ZoomViewModel : NotificationObject
    {

        public event Action<BlockSize> OnZoomChanged;

        private string _zoomText;

        public string ZoomText
        {
            get { return _zoomText; }
            set { if (value != null) RaisePropertyChanged(ref _zoomText, value + "%"); }
        }

        private BlockSize _zoom;
        public BlockSize Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = value;
                ZoomText = CreateZoomText(value);
                OnZoomChanged?.Invoke(_zoom);
                Save();
            }
        }
        
        public RelayCommand ZoomInCommand { get;  }
        public RelayCommand ZoomOutCommand { get; }


        public ZoomViewModel()
        {
            ZoomInCommand  = new RelayCommand(ZoomInExecute, ZoomInCanExecute);
            ZoomOutCommand = new RelayCommand(ZoomOutExecute, ZoomOutCanExecute);
        }

        public void Init()
        {
            Load();
        }

        private string CreateZoomText(BlockSize size)
        {
            var current = size.GetPixelSize();
            var normal  = BlockSize.Zoom100.GetPixelSize();
            var zoom    = (double) current * 100 / normal;
            var zoomInt = (int) zoom;

            return zoomInt.ToString(CultureInfo.InvariantCulture);
        }

        private bool ZoomOutCanExecute(object obj)
        {
            return Zoom > BlockSize.Zoom5;
        }

        private bool ZoomInCanExecute(object obj)
        {
            return Zoom < BlockSize.Zoom250;
        }

        private void ZoomInExecute(object obj)
        {
            if (!ZoomInCanExecute(null))
                return;

            Zoom += 1;
        }

        private void ZoomOutExecute(object obj)
        {
            if (!ZoomOutCanExecute(null))
                return;

            Zoom -= 1;
        }

        private void Save()
        {
            try
            {
                Settings.Default["Zoom"] = (int) Zoom;
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        private void Load()
        {
            try
            {
                Zoom = (BlockSize)(Settings.Default["Zoom"] ?? BlockImages.DEFAULT_BLOCK_SIZE);
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

    }
}
