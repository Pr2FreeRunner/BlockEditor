using LevelModel.Models.Components;
using System.Collections.Generic;

namespace BlockEditor.Models
{

    public class UniqueBlocks
    {
        public SimpleBlock Player1 { get; set; }
        public SimpleBlock Player2 { get; set; }
        public SimpleBlock Player3 { get; set; }
        public SimpleBlock Player4 { get; set; }

        public UniqueBlocks()
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
    }
}
