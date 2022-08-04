using LevelModel.Models;
using LevelModel.Models.Components;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows.Media;

using static BlockEditor.Models.BlockImages;

namespace BlockEditor.Models
{

    public class Blocks
    {

        public const int SIZE = 2_000;

        private int?[,] _blocks;

        public bool Overwrite { get; set; }

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


        public void Add(MyPoint p, int id)
        {
            if(p.X < 0 || p.Y < 0)
                return;

            if(p.X >= SIZE || p.Y >= (SIZE - 1))
                return;

            

            if(Block.IsStartBlock(id))
                AddStartBlock(p, id);
            else
            {
                if (!IsPositionOccupied(p))
                    _blocks[p.X, p.Y] = id;
            }
        }

        private void AddStartBlock(MyPoint p, int blockid)
        {
            foreach (var startBlock in StartBlocks.GetBlocks())
            {
                if(blockid != startBlock.ID)
                    continue;

                startBlock.Position = p;
                return;
            }
        }

        public void Delete(MyPoint p)
        {
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
