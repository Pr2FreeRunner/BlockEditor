namespace BlockEditor.Models
{
    public class SimpleBlock
    {

        public MyPoint? Position { get; set; }

        public int ID { get; set; }

        public SimpleBlock(int id)
        {
            ID = id;
        }

        public SimpleBlock(int id, MyPoint p)
        {
            ID = id;
            Position = p;
        }

        public SimpleBlock(int id, int x, int y)
        {
            ID = id;
            Position = new MyPoint(x, y);

        }
    }
}
