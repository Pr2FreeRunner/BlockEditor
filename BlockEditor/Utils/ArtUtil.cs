using BlockEditor.Models;
using LevelModel.Models.Components.Art;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockEditor.Utils
{
    public static class ArtUtil
    {

        public static List<DrawArt> GetArtInside(List<DrawArt> art, MyRegion region)
        {
            // optmized version of region.IsInside()

            var result = new List<DrawArt>();

            if (art == null || region == null || !region.IsComplete())
                return result;

            var start = region.Start.Value;
            var end = region.End.Value;

            foreach (var a in art)
            {
                var x = a.X / 30;
                var y = a.Y / 30;

                if (x < start.X || x >= end.X)
                    continue;

                if (y < start.Y || y >= end.Y)
                    continue;


                result.Add(a);
            }

            return result;
        }
    }
}
