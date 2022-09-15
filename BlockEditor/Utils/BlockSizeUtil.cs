using System;
using System.Collections.Generic;

using static BlockEditor.Models.BlockImages;

namespace BlockEditor.Utils
{
    public static class BlockSizeUtil
    {

        public const int DEFAULT_BLOCK_SIZE = 30;

        public static IEnumerable<BlockSize> GetAll()
        {
            foreach (var e in Enum.GetValues(typeof(BlockSize)))
                yield return (BlockSize)e;
        }

        public static double GetScale(this BlockSize size)
        {
            switch (size)
            {
                case BlockSize.Zoom5: return 0.05;

                case BlockSize.Zoom10: return 0.10;

                case BlockSize.Zoom25: return 0.25;

                case BlockSize.Zoom50: return 0.5;

                case BlockSize.Zoom75: return 0.75;

                case BlockSize.Zoom90: return 0.90;

                case BlockSize.Zoom100: return 1.00;
                
                case BlockSize.Zoom110: return 1.10;

                case BlockSize.Zoom125: return 1.25;

                case BlockSize.Zoom150: return 1.50;

                case BlockSize.Zoom175: return 1.75;

                case BlockSize.Zoom200: return 2.00;

                case BlockSize.Zoom250: return 2.50;

                default: return 1.0;
            }
        }

        public static int GetPixelSize(this BlockSize size)
        {
            return (int)(DEFAULT_BLOCK_SIZE * GetScale(size));
        }

    }
}
