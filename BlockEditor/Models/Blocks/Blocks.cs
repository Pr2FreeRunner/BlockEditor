using LevelModel.Models.Components;
using System.Collections.Generic;

namespace BlockEditor.Models
{
    public class Blocks
    {

        public const int SIZE = 2_000;
        public const int LIMIT = 50_000;

        private int?[,] _blocks;

        public bool Overwrite { get; set; }
        public int BlockCount;
        public UniqueBlocks StartBlocks { get; }

        public Blocks()
        {
            _blocks       = new int?[SIZE, SIZE];
            StartBlocks = new UniqueBlocks();
            Overwrite      = true;
        }

        private bool IsPositionOccupied(MyPoint p)
        {
            if (_blocks[p.X, p.Y] != null)
            {
                if (!Overwrite)
                    return true;

                Delete(p);
            }

            return false;
        }

        public int? GetBlockId(int x, int y)
        {
            if(x < 0 || y < 0)
                return null;

            if(x >= SIZE || y >= SIZE)
                return null;

            return _blocks[x, y];
        }

        public int? GetBlockId(MyPoint? point)
        {
            if(point == null)
                return null;

            return GetBlockId(point.Value.X, point.Value.Y);
        }


        public void Add(MyPoint p, int id)
        {
            if(p.X < 0 || p.Y < 0)
                return;

            if(p.X >= SIZE || p.Y >= (SIZE - 1))
                return;       

            if(Block.IsStartBlock(id))
            { 
                AddStartBlock(p, id);
            }
            else
            {
                if (!IsPositionOccupied(p))
                {
                    if(BlockCount >= LIMIT)
                        throw new BlockLimitException();

                    _blocks[p.X, p.Y] = id;
                    BlockCount++;
                }
            }
        }

        private void AddStartBlock(MyPoint p, int blockid)
        {
            foreach (var startBlock in StartBlocks.GetBlocks())
            {
                if(blockid != startBlock.ID)
                    continue;

                if(startBlock.Position == null)
                {
                    if (BlockCount >= LIMIT)
                        throw new BlockLimitException();

                    BlockCount++;
                }

                startBlock.Position = p;
                return;
            }
        }

        public void Delete(MyPoint p)
        {
            if(GetBlockId(p.X, p.Y) != null)
                BlockCount--;

            _blocks[p.X, p.Y] = null;
        }

        public MyPoint? GetStartPosition()
        {
            return StartBlocks?.Player1?.Position;
        }

        public IEnumerable<SimpleBlock> GetBlocks()
        {
            if(_blocks == null)
                yield break;

            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    var id = _blocks[x, y];

                    if(id == null)
                        continue;

                    yield return new SimpleBlock(id.Value, new MyPoint(x, y));
                }
            }


        }

    }
}
