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
using System.Linq;
using LevelModel.Models.Components;

using static System.Net.WebRequestMethods;
using static BlockEditor.Models.BlockImages;

namespace BlockEditor.Models
{


    public static class BlockImages
    {
        public enum BlockSize { Zoom5, Zoom10, Zoom25, Zoom50, Zoom75, Zoom90, Zoom100, Zoom110, Zoom125, Zoom150, Zoom175, Zoom200, Zoom250 };

        public const BlockSize DEFAULT_BLOCK_SIZE = BlockSize.Zoom125;

        private static Dictionary<BlockSize, Dictionary<int, BlockImage>> _images;
        private static Dictionary<BlockSize, BlockImage> _unknownBlocks;
        public const int _unknownID = 99;
        private static Dictionary<BlockSize, Dictionary<string, BlockImage>> _teleportImages;


        public static void Init()
        {
            _images = new Dictionary<BlockSize, Dictionary<int, BlockImage>>();
            _unknownBlocks = new Dictionary<BlockSize, BlockImage>();
            _teleportImages = new Dictionary<BlockSize, Dictionary<string, BlockImage>>();

            foreach (var size in BlockSizeUtil.GetAll())
            {
                _images.Add(size, new Dictionary<int, BlockImage>());
                _teleportImages.Add(size, new Dictionary<string, BlockImage>());
            }

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

        private static List<Tuple<BlockSize, BlockImage>> GetImages(string filepath)
        {
            var fallback = new List<Tuple<BlockSize, BlockImage>>();
            var src = GetImage(filepath);

            if (src == null)
                return fallback;

            var filename = Path.GetFileNameWithoutExtension(filepath);

            if (!int.TryParse(filename, NumberStyles.Any, CultureInfo.InvariantCulture, out var id))
                return fallback;

            return GetImages(src, id).ToList();
        }

        private static IEnumerable<Tuple<BlockSize, BlockImage>> GetImages(BitmapImage src, int id)
        {
            foreach (var e in BlockSizeUtil.GetAll())
            {
                var image = Resize(e.GetPixelSize(), src);
                var bitmap = ToBitmap(image);
                var semi = ToSemiTransparentBitmap(bitmap, 0.5f);
                var block = new BlockImage { ID = id, Image = image, Bitmap = bitmap, SemiTransparentBitmap = semi };

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

        public static BlockImage GetTeleportImageBlock(BlockSize size, string option)
        {
            if (_teleportImages != null && !string.IsNullOrWhiteSpace(option))
            {
                if (_teleportImages.TryGetValue(size, out var dic))
                {
                    if (dic.TryGetValue(option, out var block))
                        return block;
                }
            }

            return GetImageBlock(size, Block.TELEPORT);
        }

        private static BitmapImage ToBitmapImage(Bitmap bitmap)
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

        private static bool TeleportBlockExist(string option)
        {
            if(_teleportImages == null)
                return false;

            if (_teleportImages.TryGetValue(BlockSize.Zoom100, out var dic))
                if (dic.TryGetValue(option, out _))
                    return true;

            return false;
        }

        public static void AddTeleportBlock(string option)
        {
            if(string.IsNullOrWhiteSpace(option))
                return;

            if(_teleportImages == null)
                return;

            if(TeleportBlockExist(option))
                return;

            var teleport = GetImageBlock(BlockSize.Zoom100, Block.TELEPORT);

            if(teleport == null)
                return;

            var image    = (Bitmap) teleport.Bitmap.Clone();
            var color    = ColorUtil.GetColorFromBlockOption(option);
            var graphics = Graphics.FromImage(image);
            
            if (color == null || graphics == null)
                return;

            // fill
            var pen  = new System.Drawing.Pen(color.Value, 6);
            var size = new Size((int) (image.Width * 0.5) + 1, (int)(image.Height * 0.5));
            var loc  = new System.Drawing.Point((int) (image.Width * 0.2 + 1), (int) (image.Height * 0.2));
            var rec  = new Rectangle(loc, size);
            graphics.DrawEllipse(pen, rec);

            var outlineColor = System.Drawing.Color.Black;
            if (color.Value.R * 0.30 + color.Value.G * 0.50 + color.Value.B * 0.2 < 35)
                outlineColor = System.Drawing.Color.White;

            // outter
            var pen2 = new System.Drawing.Pen(outlineColor, 2);
            var size2 = new Size((int)(image.Width * 0.5 + 7), (int)(image.Height * 0.5) + 7);
            var loc2 = new System.Drawing.Point((int)(image.Width * 0.2 - 2), (int)(image.Height * 0.2 - 3));
            var rec2 = new Rectangle(loc2, size2);
            graphics.DrawEllipse(pen2, rec2);

            // inner
            var pen3 = new System.Drawing.Pen(outlineColor, 2);
            var size3 = new Size((int)(image.Width * 0.5 - 6), (int)(image.Height * 0.5) - 6);
            var loc3 = new System.Drawing.Point((int)(image.Width * 0.2 + 5), (int)(image.Height * 0.2 + 4));
            var rec3 = new Rectangle(loc3, size3);
            graphics.DrawEllipse(pen3, rec3);

            foreach (var item in GetImages(ToBitmapImage(image), Block.TELEPORT))
            {
                if (item == null)
                    continue;

                var blockSize = item.Item1;
                var block = item.Item2;

                if (block == null)
                    continue;

                var white = GetImageBlock(blockSize, Block.BASIC_WHITE);

                if(white != null && white.Bitmap.Width != block.Bitmap.Width && block.Bitmap.Width != 0)
                    block.Bitmap = new Bitmap(block.Bitmap, new Size(white.Bitmap.Width, white.Bitmap.Height));

                if (_teleportImages.TryGetValue(blockSize, out var dic))
                    dic.Add(option, item.Item2);
            }
        }
    }
}
