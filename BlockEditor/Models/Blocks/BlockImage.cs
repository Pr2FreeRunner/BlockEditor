using System.Drawing;
using System.Windows.Media.Imaging;

namespace BlockEditor.Models
{
    public class BlockImage
    {

        public int ID { get; set; }

        public Bitmap Bitmap;

        public Bitmap SemiTransparentBitmap { get; set; }

        public BitmapSource Image { get; set; }

    }
}
