using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media.Imaging;
using BlockEditor.Helpers;
using System.Drawing;
using System.Windows.Media;

namespace BlockEditor.Models
{
    public static class BlockImages
    {

        private static Dictionary<BlockSize, Dictionary<int, BlockImage>> _images;
        private static Dictionary<BlockSize, BlockImage> _unknownBlocks;
        public const int _unknownID = 99;

        public enum BlockSize { SuperSmall, VerySmall, Small, Normal, Big, VeryBig, SuperBig };


        public const int DEFAULT_SIZE = 40;

        public static void Init()
        {
            _images = new Dictionary<BlockSize, Dictionary<int, BlockImage>>();
            _unknownBlocks = new Dictionary<BlockSize, BlockImage>();

            foreach (var size in GetBlockSizes())
                _images.Add(size, new Dictionary<int, BlockImage>());

            LoadImages();
        }

        private static IEnumerable<BlockSize> GetBlockSizes()
        {
            foreach (var e in Enum.GetValues(typeof(BlockSize)))
                yield return (BlockSize)e;
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

        private static IEnumerable<Tuple<BlockSize, BlockImage>> GetAllSizedImages(string filepath)
        {
            var src = GetImage(filepath);

            if (src == null)
                yield break;

            var filename = Path.GetFileNameWithoutExtension(filepath);

            if (!int.TryParse(filename, NumberStyles.Any, CultureInfo.InvariantCulture, out var id))
                yield break;

            foreach (var e in GetBlockSizes())
            {
                var image = Resize(e.GetPixelSize(), src);
                var bitmap = ToBitmap(image);
                var block = new BlockImage { ID = id, Image = image, Bitmap = bitmap };

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

        public static int GetPixelSize(this BlockSize size)
        {
            switch (size)
            {
                case BlockSize.SuperSmall : return 4;

                case BlockSize.VerySmall: return 10;

                case BlockSize.Small: return 20;

                case BlockSize.Big: return 60;

                case BlockSize.VeryBig: return 80;

                case BlockSize.SuperBig: return 100;

                default: return 40;
            }
        }

        public static BlockImage GetImageBlock(BlockSize size, int id)
        {
            if (_images == null)
                return null;

            if (!_images.TryGetValue(size, out var dic))
                return null;

            if (dic.TryGetValue(id, out var block))
                return block;

            if(_unknownBlocks.TryGetValue(size, out var unknownBlock))
                return unknownBlock;

            return null;
        }

        public static IEnumerable<BlockImage> GetImageBlocks(BlockSize size)
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
                foreach (var item in GetAllSizedImages(file))
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
