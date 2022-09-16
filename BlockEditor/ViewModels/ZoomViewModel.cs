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

        public BlockSize Zoom
        {
            get
            {
                return MySettings.Zoom;
            }
            set
            {
                MySettings.Zoom = value;
                ZoomText = CreateZoomText(value);
                OnZoomChanged?.Invoke(value);
            }
        }
        
        public RelayCommand ZoomInCommand { get;  }
        public RelayCommand ZoomOutCommand { get; }


        public ZoomViewModel()
        {
            ZoomInCommand  = new RelayCommand(ZoomInExecute, ZoomInCanExecute);
            ZoomOutCommand = new RelayCommand(ZoomOutExecute, ZoomOutCanExecute);
        }


        private string CreateZoomText(BlockSize size)
        {
            //var current = size.GetPixelSize();
            var current = size.GetScale();
            var normal  = BlockSize.Zoom100.GetPixelSize();
            var zoom = (double)current * 100;// / normal;
            var zoomInt = (int) zoom;

            return zoomInt.ToString(CultureInfo.InvariantCulture);
        }

        private bool ZoomOutCanExecute(object obj)
        {
            return Zoom > BlockSize.Zoom5;
        }

        private bool ZoomInCanExecute(object obj)
        {
            return Zoom < BlockSize.Zoom300;
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
