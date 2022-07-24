using System;
using System.Collections.Generic;
using System.Text;

namespace BlockEditor.Models
{
    class Map
    {

        public Blocks Blocks { get; set; }

        public Map()
        {
            Blocks = new Blocks();
        }
    }
}
