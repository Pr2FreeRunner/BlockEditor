﻿using BlockEditor.Models;
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

        public static void CreateRelativePosition(List<TextArt> arts)
        {
            if (arts == null)
                return;

            var previousX = 0;
            var previousY = 0;

            foreach (var a in arts)
            {
                var tempX = a.X;
                var tempY = a.Y;

                a.X -= previousX;
                a.Y -= previousY;

                previousX = tempX;
                previousY = tempY;
            }
        }

        public static void CreateAbsolutePosition(List<TextArt> arts)
        {
            if (arts == null)
                return;

            var x = 0;
            var y = 0;

            foreach (var a in arts)
            {
                x += a.X;
                y += a.Y;

                a.X = x;
                a.Y = y;
            }
        }
    }
}
