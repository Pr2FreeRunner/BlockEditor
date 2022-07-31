using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace BlockEditor.Models
{

    public class GameImage : IDisposable
    {

        public int Width { get; }
        public int Height { get; }

        private readonly int _stride;
        private readonly int _length;
        private readonly IntPtr _location;

        public GameImage(int width, int height)
        {
            _length   = width * height * 4;
            _location = Marshal.AllocCoTaskMem(_length);
            _stride   = width * 4;
            Width     = width;
            Height    = height;

            Clear(Color.Black);
        }

        public BitmapImage GetImage()
        {
            var bitmap = GetBitmap();
            var image  = ToImage(bitmap);

            return image;
        }

        public Bitmap GetBitmap()
        {
            if (Width == 0 || Height == 0)
                return null;

            return new Bitmap(Width, Height, _stride, PixelFormat.Format32bppPArgb, _location);
        }

        BitmapImage ToImage(Bitmap bitmap)
        {
            if(bitmap == null)
                return null;

            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public void Dispose()
        {
            Marshal.FreeCoTaskMem(_location);
        }

        public void Clear(Color color)
        {
            int locWidth = Width; 
            int val      = color.ToArgb();
            int[] arr    = new int[locWidth];


            for (int i = 0; i < arr.Length; i++)
                arr[i] = val;

            for (int i = 0; i < _length; i += _stride)
                Marshal.Copy(arr, 0, _location + i, locWidth);
        }

        public void DrawImage(ref Bitmap img, int x, int y)
        {
            if (img.PixelFormat == PixelFormat.Format32bppArgb)
                DrawBmpImage(ref img, x, y);
            else if (img.PixelFormat == PixelFormat.Format32bppPArgb)
                DrawPngImage(ref img, x, y);
            else
                throw new Exception("The image is not in the supported format.");
        }
        private void DrawBmpImage(ref Bitmap img, int X, int Y)
        {
            int dWidth  = img.Width;
            int dHeight = img.Height;

            if (X >= Width || Y >= Height)
                return;

            int leftCutOff = 0;
            int topCutOff  = 0;

            if (X < 0)
            {
                dWidth += X;
                leftCutOff = -X;
                X = 0;
            }
            if (Y < 0)
            {
                dHeight += Y;
                topCutOff = -Y;
                Y = 0;
            }
            if (dWidth + X > Width)
                dWidth = Width - X;

            if (dHeight + Y > Height)
                dHeight = Height - Y;

            if (dWidth <= 0 || dHeight <= 0)
                return;

            // Copy rows of bytes
            int[] myBytes = new int[_stride / 4];

            // Marshal.Copy(BLoc, myBytes, 0, Stride * Height);
            BitmapData dmb = img.LockBits(new Rectangle(0, 0, 1, 1), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            IntPtr ptrMe = _location + (X * 4) + (Y * _stride);
            IntPtr ptrIt = dmb.Scan0;

            ptrIt += (leftCutOff * 4);
            ptrIt += (topCutOff * dmb.Stride);

            int ItStride = img.Width * 4;
            int copyLen  = dWidth; //*4;

            for (int iY = 0; iY < dHeight; iY++)
            {
                Marshal.Copy(ptrIt, myBytes, 0, copyLen);
                Marshal.Copy(myBytes, 0, ptrMe, copyLen);
                ptrMe += _stride;
                ptrIt += ItStride;
            }

            img.UnlockBits(dmb);
        }

        private unsafe void DrawPngImage(ref Bitmap img, int X, int Y)
        {
            int dWidth = img.Width;
            int dHeight = img.Height;

            if (X >= Width || Y >= Height)
                return;

            int leftCutOff = 0;
            int topCutOff  = 0;

            if (X < 0)
            {
                dWidth += X;
                leftCutOff = -X;
                X = 0;
            }

            if (Y < 0)
            {
                dHeight += Y;
                topCutOff = -Y;
                Y = 0;
            }

            if (dWidth + X > Width)
                dWidth = Width - X;

            if (dHeight + Y > Height)
                dHeight = Height - Y;

            if (dWidth <= 0 || dHeight <= 0)
                return;

            var dmb      = img.LockBits(new Rectangle(0, 0, 1, 1), ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
            IntPtr ItPtr = dmb.Scan0;
            IntPtr ptrMe = _location + (X * 4) + (Y * _stride);
            int ItStride = img.Width * 4;

            ItPtr += leftCutOff * 4;
            ItPtr += topCutOff * dmb.Stride;

            for (int iY = 0; iY < dHeight; iY++)
            {
                byte* pMe = (byte*)ptrMe;
                byte* pIt = (byte*)ItPtr;

                for (int iX = 0; iX < dWidth; iX++)
                {
                    double a      = pIt[3] / 255.0;
                    double alpha1 = 1 - a;

                    // newC = (1 - A)*oldC + ovrCA
                    pMe[0] = (byte) (pMe[0] * alpha1 + pIt[0]);
                    pMe[1] = (byte) (pMe[1] * alpha1 + pIt[1]);
                    pMe[2] = (byte) (pMe[2] * alpha1 + pIt[2]);

                    pMe += 4;
                    pIt += 4;
                }

                ptrMe += _stride;
                ItPtr += ItStride;
            }

            img.UnlockBits(dmb);
        }
    }

}
