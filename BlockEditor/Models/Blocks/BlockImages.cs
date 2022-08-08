using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media.Imaging;
using BlockEditor.Helpers;
using System.Drawing;
using System.Windows.Media;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using BlockEditor.Utils;

namespace BlockEditor.Models
{


    public static class BlockImages
    {
        public enum BlockSize { Zoom5, Zoom10, Zoom25, Zoom50, Zoom75, Zoom90, Zoom100, Zoom110, Zoom125, Zoom150, Zoom175, Zoom200, Zoom250 };

        public const BlockSize DEFAULT_BLOCK_SIZE = BlockSize.Zoom125;

        private static Dictionary<BlockSize, Dictionary<int, BlockImage>> _images;
        private static Dictionary<BlockSize, BlockImage> _unknownBlocks;
        public const int _unknownID = 99;

        public static void Init()
        {
            _images = new Dictionary<BlockSize, Dictionary<int, BlockImage>>();
            _unknownBlocks = new Dictionary<BlockSize, BlockImage>();

            foreach (var size in BlockSizeUtil.GetAll())
                _images.Add(size, new Dictionary<int, BlockImage>());

            LoadImages();
        }

        private static string[] GetFiles()
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Blocks");
                return Directory.GetFiles(path);
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
                return null;
            }
        }

        private static BitmapImage GetImage(string filepath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filepath))
                    return null;

                return new BitmapImage(new Uri(filepath));
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
                return null;
            }
        }

        private static Bitmap ToSemiTransparentBitmap(Bitmap image, float opacity)
        {
            var colorMatrix      = new ColorMatrix();
            colorMatrix.Matrix33 = opacity;
            var imageAttributes  = new ImageAttributes();

            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            var output = new Bitmap(image.Width, image.Height);

            using (var gfx = Graphics.FromImage(output))
            {
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.DrawImage(
                    image,
                    new Rectangle(0, 0, image.Width, image.Height),
                    0,
                    0,
                    image.Width,
                    image.Height,
                    GraphicsUnit.Pixel,
                    imageAttributes);
            }

            return output;
        }

        private static IEnumerable<Tuple<BlockSize, BlockImage>> GetImages(string filepath)
        {
            var src = GetImage(filepath);

            if (src == null)
                yield break;

            var filename = Path.GetFileNameWithoutExtension(filepath);

            if (!int.TryParse(filename, NumberStyles.Any, CultureInfo.InvariantCulture, out var id))
                yield break;

            foreach (var e in BlockSizeUtil.GetAll())
            {
                var image  = Resize(e.GetPixelSize(), src);
                var bitmap = ToBitmap(image);
                var semi   = ToSemiTransparentBitmap(bitmap, 0.5f);
                var block  = new BlockImage { ID = id, Image = image, Bitmap = bitmap, SemiTransparentBitmap = semi };

                yield return new Tuple<BlockSize, BlockImage>(e, block);
            }

        }

        private static BitmapSource Resize(int size, BitmapImage src)
        {
            if (src == null || src.Width == 0 || src.Height == 0)
                return null;

            var scaleX = size / src.Width;
            var scaleY = size / src.Height;
            var image = new TransformedBitmap(src, new ScaleTransform(scaleX, scaleY));

            return image;
        }

        private static Bitmap ToBitmap(BitmapSource bitmapImage)
        {
            if (bitmapImage == null)
                return null;

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        public static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        public static BlockImage GetImageBlock(BlockSize size, int? id)
        {
            if (_images == null || id == null)
                return null;

            if (!_images.TryGetValue(size, out var dic))
                return null;

            if (dic.TryGetValue(id.Value, out var block))
                return block;

            if(_unknownBlocks.TryGetValue(size, out var unknownBlock))
                return unknownBlock;

            return null;
        }

        public static IEnumerable<BlockImage> GetAllImageBlocks(BlockSize size)
        {
            if (_images == null)
                yield break;

            if(!_images.TryGetValue(size, out var dic))
                yield break;

            foreach (var i in dic)
            {
                if (i.Value == null)
                    continue;

                yield return i.Value;
            }
        }

        private static void LoadImages()
        {
            var files = GetFiles();

            if (files == null)
                return;


            foreach (var file in files)
            {
                foreach (var item in GetImages(file))
                {
                    if (item == null)
                        continue;

                    var size  = item.Item1;
                    var block = item.Item2;

                    if (block == null)
                        continue;

                    if (item.Item2.ID == _unknownID)
                    {
                        _unknownBlocks.Add(size, block);
                    }
                    else
                    {
                        if (_images.TryGetValue(size, out var dic))
                            dic.Add(item.Item2.ID, item.Item2);
                    }
                }
            }
        }

    }
}
