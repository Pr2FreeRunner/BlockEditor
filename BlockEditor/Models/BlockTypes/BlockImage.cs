using SkiaSharp;

using System.Drawing;
using System.Windows.Media.Imaging;

namespace BlockEditor.Models
{
    public class BlockImage
    {

        public int ID { get; set; }

        public SKBitmap SKBitmap;
        public BitmapSource Image { get; set; }

    }
}
