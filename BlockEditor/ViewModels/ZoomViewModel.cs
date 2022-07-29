using BlockEditor.Helpers;
using System;
using System.Globalization;

namespace BlockEditor.ViewModels
{
    public class ZoomViewModel : NotificationObject
    {

        public event Action<double> OnZoomChanged;

        private string _zoomText;

        public string ZoomText
        {
            get { return _zoomText; }
            set { if (value != null) RaisePropertyChanged(ref _zoomText, value + "%"); }
        }

        private int _zoom;
        public int Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = value;
                ZoomText = value.ToString(CultureInfo.InvariantCulture);
                OnZoomChanged?.Invoke(_zoom / 100.0);
            }
        }
        
        public RelayCommand ZoomInCommand { get;  }
        public RelayCommand ZoomOutCommand { get; }

        private readonly int _zoomStrength;
        private readonly int _zoomDefault;

        public ZoomViewModel()
        {
            _zoomStrength = 20;
            _zoomDefault  = 100;
            Zoom          = _zoomDefault;

            ZoomInCommand  = new RelayCommand(ZoomInExecute, ZoomInCanExecute);
            ZoomOutCommand = new RelayCommand(ZoomOutExecute, ZoomOutCanExecute);

        }

        private bool ZoomOutCanExecute(object obj)
        {
            return (Zoom - _zoomStrength) > 0;
        }

        private bool ZoomInCanExecute(object obj)
        {
            return (Zoom + _zoomStrength) < 999;
        }

        private void ZoomInExecute(object obj)
        {
            if (!ZoomInCanExecute(null))
                return;

            Zoom += _zoomStrength;
        }

        private void ZoomOutExecute(object obj)
        {
            if (!ZoomOutCanExecute(null))
                return;

            Zoom -= _zoomStrength;
        }

    }
}
