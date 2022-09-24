using System;
using System.Collections.Generic;
using System.Text;

namespace BlockEditor.Utils
{
    public static class RandomUtil
    {


        private static Random _rng = new Random();


        public static int GetRandom(int min, int max)
        {
            // both min and max can be returned
            return _rng.Next(min, max + 1);
        }
    }
}
