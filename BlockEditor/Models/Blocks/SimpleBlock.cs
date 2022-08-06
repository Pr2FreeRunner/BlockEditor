namespace BlockEditor.Models
{
    public struct SimpleBlock
    {
        public static readonly SimpleBlock None = new SimpleBlock();

        public MyPoint? Position { get; set; }

        public int ID { get; set; }

        public string BlockOptions { get; set; }


        public SimpleBlock(int id)
        {
            ID = id;
            Position = null;
            BlockOptions = string.Empty;
        }

        public SimpleBlock(int id, MyPoint p)
        {
            ID = id;
            Position = p;
            BlockOptions = string.Empty;
        }

        public SimpleBlock(int id, int x, int y)
        {
            ID = id;
            Position = new MyPoint(x, y);
            BlockOptions = string.Empty;

        }


        public bool IsEmpty()
        {
            return Position == null;
        }
    }
}
