using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media.Imaging;
using BlockEditor.Helpers;

namespace BlockEditor.Models
{
    public static class BlockImages {

        private static Dictionary<int, ImageBlock> _images;
        public const int _unknownID = 99;
        private static ImageBlock _unknownBlock;

        public static void Init()
        {
            _images = new Dictionary<int, ImageBlock>();

            LoadImages();
        }

        private static string[] GetFiles()
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Blocks");
                return Directory.GetFiles(path);
            }
            catch(Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
                return null;
            }
        }

        private static ImageBlock GetImage(string filepath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filepath))
                    return null;

                var filename = Path.GetFileNameWithoutExtension(filepath);

                if (!int.TryParse(filename, NumberStyles.Any, CultureInfo.InvariantCulture, out var id))
                    return null;

                var image = new BitmapImage(new Uri(filepath));

                if (image == null)
                    return null;

                return new ImageBlock { ID = id, Source = image };
            }
            catch(Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
                return null;
            }
        }

        public static ImageBlock GetImageBlock(int id)
        {
            if(_images == null)
                return null;

            if(_images.TryGetValue(id, out var image))
                return image;

            return _unknownBlock;
        }

        public static IEnumerable<ImageBlock> GetImageBlocks()
        {
            if (_images == null)
                yield break;

            foreach (var i in _images)
            {
                if(i.Value == null)
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
                var image = GetImage(file);

                if (image == null)
                    continue;

                if(image.ID == _unknownID)
                    _unknownBlock = image;
                else
                    _images.Add(image.ID, image);
            }
        }

    }
}
