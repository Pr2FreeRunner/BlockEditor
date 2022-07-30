using BlockEditor.Helpers;
using BlockEditor.Models;
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
            }
        }
        
        public RelayCommand ZoomInCommand { get;  }
        public RelayCommand ZoomOutCommand { get; }


        public ZoomViewModel()
        {
            Zoom = BlockSize.Zoom100;

            ZoomInCommand  = new RelayCommand(ZoomInExecute, ZoomInCanExecute);
            ZoomOutCommand = new RelayCommand(ZoomOutExecute, ZoomOutCanExecute);

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
            return Zoom > BlockSize.Zoom10;
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

    }
}
