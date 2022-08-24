using LevelModel.Models.Components;
using System;

namespace BlockEditor.Models
{
    public struct SimpleBlock
    {
        public static readonly SimpleBlock None = new SimpleBlock();

        public MyPoint? Position { get; set; }

        public int ID { get; set; }

        public string Options { get; set; }


        public SimpleBlock(int id)
        {
            ID = id;
            Position = null;
            Options = string.Empty;
        }

        public SimpleBlock(int id, MyPoint p)
        {
            ID = id;
            Position = p;
            Options = string.Empty;
        }

        public SimpleBlock(int id, int x, int y)
        {
            ID = id;
            Position = new MyPoint(x, y);
            Options = string.Empty;

        }

        public SimpleBlock(int id, int x, int y, string options)
        {
            ID = id;
            Position = new MyPoint(x, y);
            Options = options;
        }

        public SimpleBlock(int id, MyPoint p, string options)
        {
            ID = id;
            Position = p;
            Options = options;
        }

        public bool IsEmpty()
        {
            return Position == null;
        }

        internal bool IsItem()
        {
            if(IsEmpty())
                return false;

            return ID == Block.ITEM_BLUE || ID == Block.ITEM_RED;
        }

    }
}
