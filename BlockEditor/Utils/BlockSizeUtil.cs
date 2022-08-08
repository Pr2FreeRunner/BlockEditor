using System;
using System.Collections.Generic;

using static BlockEditor.Models.BlockImages;

namespace BlockEditor.Utils
{
    public static class BlockSizeUtil
    {

        public const int DEFAULT_BLOCK_SIZE = 40;

        public static IEnumerable<BlockSize> GetAll()
        {
            foreach (var e in Enum.GetValues(typeof(BlockSize)))
                yield return (BlockSize)e;
        }

        public static int GetPixelSize(this BlockSize size)
        {
            switch (size)
            {
                case BlockSize.Zoom5: return (int)(DEFAULT_BLOCK_SIZE * 0.05);

                case BlockSize.Zoom10: return (int)(DEFAULT_BLOCK_SIZE * 0.10);

                case BlockSize.Zoom25: return (int)(DEFAULT_BLOCK_SIZE * 0.25);

                case BlockSize.Zoom50: return (int)(DEFAULT_BLOCK_SIZE * 0.5);

                case BlockSize.Zoom75: return (int)(DEFAULT_BLOCK_SIZE * 0.75);

                case BlockSize.Zoom90: return (int)(DEFAULT_BLOCK_SIZE * 0.90);

                case BlockSize.Zoom100: return (int)(DEFAULT_BLOCK_SIZE * 1.00);
                
                case BlockSize.Zoom110: return (int)(DEFAULT_BLOCK_SIZE * 1.10);

                case BlockSize.Zoom125: return (int)(DEFAULT_BLOCK_SIZE * 1.25);

                case BlockSize.Zoom150: return (int)(DEFAULT_BLOCK_SIZE * 1.50);

                case BlockSize.Zoom175: return (int)(DEFAULT_BLOCK_SIZE * 1.75);

                case BlockSize.Zoom200: return (int)(DEFAULT_BLOCK_SIZE * 2.00);

                case BlockSize.Zoom250: return (int)(DEFAULT_BLOCK_SIZE * 2.50);

                default: return DEFAULT_BLOCK_SIZE;
            }
        }

    }
}
