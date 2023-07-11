using System;
using System.Windows;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;

namespace BlockEditor.Views.Controls
{
    public partial class MyTabControl : UserControl
    {
        public Guid TabID { get;  }

        public MapControl MapControl { get; }

        public event Action<MyTabControl> OnClick;
        public event Action<MyTabControl> OnClose;

        private int _nr;

        public MyTabControl(int nr)
        {
            _nr = nr;
            TabID = Guid.NewGuid();
            InitializeComponent();
            MapControl = new MapControl();
            tbTitle.Text = GetDefaultTitle();
        }


        public string GetDefaultTitle()
        {
            var title = "Untitled";

            if(_nr > 1)
                return  title + "  (" + _nr.ToString(CultureInfo.InvariantCulture) + ")";

            return title;
        }

        public void OnSelection(bool selected)
        {
            if(selected)
            {
                MapControl.ViewModel.Game.Engine.Pause = false;
                MainGrid.Background = GetSelectedColor();
                separator.Visibility = Visibility.Collapsed;
            }
            else
            {
                MapControl.ViewModel.Game.Engine.Pause = true;
                MainGrid.Background = new SolidColorBrush(Colors.White);
                separator.Visibility = Visibility.Visible;
            }
        }

        private SolidColorBrush GetSelectedColor()
        {
            var color =  (Color)ColorConverter.ConvertFromString("#36b736");

            return new SolidColorBrush(color);
        }

        private void Map_OnTitleChanged(string title)
        {
            if(string.IsNullOrWhiteSpace(title))
                return;

            Application.Current?.Dispatcher?.Invoke(() => tbTitle.Text = title);
        }

        private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OnClose?.Invoke(this);
        }

        private void UserControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnClick?.Invoke(this);
        }

    }
}
