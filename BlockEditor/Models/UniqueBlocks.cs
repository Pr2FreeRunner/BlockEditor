using System.Collections.Generic;

namespace BlockEditor.Models
{
    public class UniqueBlock
    {
        public MyPoint? Position { get; set; }
        public int ID { get; set; }

        public UniqueBlock(int id)
        {
            ID = id;
        }
    }

    public class UniqueBlocks
    {
        public UniqueBlock Player1 { get; set; }
        public UniqueBlock Player2 { get; set; }
        public UniqueBlock Player3 { get; set; }
        public UniqueBlock Player4 { get; set; }

        public UniqueBlocks()
        {
            Player1 = new UniqueBlock(111); 
            Player2 = new UniqueBlock(112);
            Player3 = new UniqueBlock(113);
            Player4 = new UniqueBlock(114);
        }

        public IEnumerable<UniqueBlock> GetBlocks()
        {
            yield return Player1;
            yield return Player2;
            yield return Player3;
            yield return Player4;
        }
    }
}
