using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Media.Imaging;
using BlockEditor.Helpers;

namespace BlockEditor.Models
{
    public static class BlockImages {

        public static readonly Dictionary<int, ImageBlock> Images;

        static BlockImages() {
            Images = new Dictionary<int, ImageBlock>();
            
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

                Images.Add(image.ID, image);
            }
        }

    }
}
