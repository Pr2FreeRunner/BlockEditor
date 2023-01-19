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
using SkiaSharp;
using SkiaSharp.Views.WPF;

namespace BlockEditor.Models
{


    public static class BlockImages
    {
        public enum BlockSize { Zoom5, Zoom10, Zoom20, Zoom40, Zoom60, Zoom80, Zoom100, 
                    Zoom120, Zoom140, Zoom160, Zoom180, Zoom200, Zoom220, Zoom240, Zoom260, Zoom280, Zoom300 };

        public const BlockSize DEFAULT_BLOCK_SIZE = BlockSize.Zoom140;

        private static Dictionary<BlockSize, Dictionary<int, BlockImage>> _images;
        private static Dictionary<BlockSize, BlockImage> _unknownBlocks;
        public const int _unknownID = 99;
        private static Dictionary<BlockSize, Dictionary<string, BlockImage>> _teleportImages;
        private static SKBitmap _teleportMask;

        public static SKImage BlocksSheet { get; private set; }

        public static void Init()
        {
            _images = new Dictionary<BlockSize, Dictionary<int, BlockImage>>();
            _unknownBlocks = new Dictionary<BlockSize, BlockImage>();
            _teleportImages = new Dictionary<BlockSize, Dictionary<string, BlockImage>>();
            _teleportMask = SKBitmap.Decode(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "132mask.png"));
            BlocksSheet = SKImage.FromEncodedData(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "BlocksSheet.png"));

            foreach (var size in BlockSizeUtil.GetAll())
            {
                _images.Add(size, new Dictionary<int, BlockImage>());
                _teleportImages.Add(size, new Dictionary<string, BlockImage>());
            }

            LoadImages();
        }

        public static SKRect GetSpriteFromId(int id)
        {
            const int SheetColumns = 10;
            const int BlockSize = 30;

            if (id >= 100)
                id -= 100;

            var y = (id / SheetColumns) * BlockSize;
            var x = (id % SheetColumns) * BlockSize;

            return new SKRect(x, y, x + BlockSize, y + BlockSize);
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

        public static BitmapSource GetBlockBitmapSourceFromId(int id)
        {
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Blocks", id+".bmp");
            return new BitmapImage(new Uri(filepath));
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
                var skBitmap = image.ToSKBitmap();
                var block = new BlockImage { ID = id, Image = image, SKBitmap = skBitmap };

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

        private static BitmapImage ToBitmapImage(SKBitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {

                bitmap.Encode(memory, SKEncodedImageFormat.Png, 100);
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

            if (_teleportMask == null)
                return;

            var color    = ColorUtil.GetColorFromBlockOption(option);
            var teleBmp = new SKBitmap(_teleportMask.Width, _teleportMask.Height);
            using (var canvas = new SKCanvas(teleBmp))
            {
                var skcolor = ColorUtil.ToSkColor(color);
                canvas.DrawColor(skcolor);
                canvas.DrawBitmap(_teleportMask, new SKPoint());
            }

            foreach (var item in GetImages(ToBitmapImage(teleBmp), Block.TELEPORT))
            {
                if (item == null)
                    continue;

                var blockSize = item.Item1;
                var block = item.Item2;

                if (block == null)
                    continue;

                var white = GetImageBlock(blockSize, Block.BASIC_WHITE);

                if (white != null && white.SKBitmap.Width != block.SKBitmap.Width && block.SKBitmap.Width != 0)
                    block.SKBitmap = teleBmp;

                if (_teleportImages.TryGetValue(blockSize, out var dic))
                    dic.Add(option, item.Item2);
            }
        }
    }
}
