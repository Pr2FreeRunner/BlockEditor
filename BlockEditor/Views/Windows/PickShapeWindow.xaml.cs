using System.Windows;
using System.Windows.Input;
using static BlockEditor.Utils.ShapeBuilderUtil;

namespace BlockEditor.Views.Windows
{

    public partial class PickShapeWindow : Window
    {
        public ShapeType Result { get; private set; }
        public bool Fill { get; set; }

        public PickShapeWindow(ShapeType fallback, bool fill)
        {
            InitializeComponent();
            Result = fallback;
            FillCheckbox.IsChecked = fill;
        }

        private void btnRectangle_Click(object sender, RoutedEventArgs e)
        {
            Result = ShapeType.Rectangle;
            DialogResult = true;
            Close();
        }

        private void btnSquare_Click(object sender, RoutedEventArgs e)
        {
            Result = ShapeType.Square;
            DialogResult = true;
            Close();
        }

        private void btnCircle_Click(object sender, RoutedEventArgs e)
        {
            Result = ShapeType.Circle;
            DialogResult = true;
            Close();
        }

        private void btnEllipse_Click(object sender, RoutedEventArgs e)
        {
            Result = ShapeType.Ellipse;
            DialogResult = true;
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }

            if (e.Key == Key.Space)
            {
                DialogResult = true;
                Close();
            }
        }

        private void FillCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            Fill = !Fill;
        }
    }
}
