using LevelModel.Models.Components;
using System.Collections.Generic;

namespace BlockEditor.Models
{

    public class StartBlocks
    {
        public SimpleBlock Player1 { get; set; }
        public SimpleBlock Player2 { get; set; }
        public SimpleBlock Player3 { get; set; }
        public SimpleBlock Player4 { get; set; }


        public StartBlocks()
        {
            Player1 = new SimpleBlock(Block.START_BLOCK_P1); 
            Player2 = new SimpleBlock(Block.START_BLOCK_P2);
            Player3 = new SimpleBlock(Block.START_BLOCK_P3);
            Player4 = new SimpleBlock(Block.START_BLOCK_P4);
        }


        public IEnumerable<SimpleBlock> GetBlocks()
        {
            yield return Player1;
            yield return Player2;
            yield return Player3;
            yield return Player4;
        }

        public MyPoint? GetPosition(int id)
        {
            foreach (var startBlock in GetBlocks())
            {
                if (startBlock.ID != id)
                    continue;

                return startBlock.Position;
            }

            return null;
        }

        public SimpleBlock GetBlock(int x, int y)
        {
            return GetBlock(new MyPoint(x, y));
        }
      
        public SimpleBlock GetBlock(MyPoint? p)
        {
            if(p == null)
                return SimpleBlock.None;

            foreach(var startBlock in GetBlocks())
            {
                if(startBlock.IsEmpty())
                    continue;

                if(startBlock.Position != p.Value)
                    continue;

                return startBlock;
            }

            return SimpleBlock.None;
        }

        public List<SimpleBlock> GetBlocks(int x, int y)
        {
            return GetBlocks(new MyPoint(x, y));
        }

        public List<SimpleBlock> GetBlocks(MyPoint? p)
        {
            var result = new List<SimpleBlock>();

            if (p == null)
                return result;

            foreach (var startBlock in GetBlocks())
            {
                if (startBlock.IsEmpty())
                    continue;

                if (startBlock.Position != p.Value)
                    continue;

                result.Add(startBlock);
            }

            return result;
        }

        public void Remove(int id)
        {
            if(Player1.ID == id)
                Player1 = new SimpleBlock(Block.START_BLOCK_P1);

            if (Player2.ID == id)
                Player2 = new SimpleBlock(Block.START_BLOCK_P2);

            if (Player3.ID == id)
                Player3 = new SimpleBlock(Block.START_BLOCK_P3);

            if (Player4.ID == id)
                Player4 = new SimpleBlock(Block.START_BLOCK_P4);
        }

    }
}
