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
    }
}
