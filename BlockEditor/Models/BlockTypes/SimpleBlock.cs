using BlockEditor.Utils;

using LevelModel.Models.Components;

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
            Options = options ?? string.Empty;
        }

        public SimpleBlock Move(int x, int y)
        {
            return new SimpleBlock(ID, x, y, Options);
        }

        public SimpleBlock Move(MyPoint p)
        {
            return Move(p.X, p.Y);
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
