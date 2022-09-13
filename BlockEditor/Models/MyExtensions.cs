using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockEditor.Models
{
    public static class MyExtensions
    {

        public static bool IsComplete(this MyRegion region)
        {
            if (region == null)
                return false; ;

            if (region.Point1 == null)
                return false;

            if (region.Point2 == null)
                return false;

            return true;
        }

        public static List<SimpleBlock> RemoveEmpty(this IEnumerable<SimpleBlock> blocks)
        {
            var result = new List<SimpleBlock>();

            if(blocks == null)
                return result;

            return blocks.Where(b => !b.IsEmpty()).ToList();
        }

        public static bool AnyBlocks(this IEnumerable<SimpleBlock> blocks)
        {
            var result = new List<SimpleBlock>();

            if (blocks == null)
                return false;

            return blocks.Any(b => !b.IsEmpty());
        }
    }
}
