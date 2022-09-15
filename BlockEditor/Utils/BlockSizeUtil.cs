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
                case BlockSize.Zoom5:  return 0.05;

                case BlockSize.Zoom10: return 0.1;

                case BlockSize.Zoom20: return 0.2;

                case BlockSize.Zoom40: return 0.4;

                case BlockSize.Zoom60: return 0.6;

                case BlockSize.Zoom80: return 0.8;

                case BlockSize.Zoom100: return 1.0;
                
                case BlockSize.Zoom120: return 1.2;

                case BlockSize.Zoom140: return 1.4;

                case BlockSize.Zoom160: return 1.6;

                case BlockSize.Zoom180: return 1.8;

                case BlockSize.Zoom200: return 2.0;

                case BlockSize.Zoom220: return 2.2;

                case BlockSize.Zoom240: return 2.4;

                case BlockSize.Zoom260: return 2.6;

                case BlockSize.Zoom280: return 2.8;

                case BlockSize.Zoom300: return 3.0;

                default: return 1.0;
            }
        }

        public static int GetPixelSize(this BlockSize size)
        {
            return (int)(DEFAULT_BLOCK_SIZE * GetScale(size));
        }

    }
}
