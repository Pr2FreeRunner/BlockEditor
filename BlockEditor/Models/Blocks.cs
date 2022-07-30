using LevelModel.Models;
using LevelModel.Models.Components;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static BlockEditor.Models.BlockImages;

namespace BlockEditor.Models
{

    public class Blocks
    {

        public const int SIZE = 2_000;

        private int?[,] _blocks;

        public bool Override { get; set; }

        private readonly UniqueBlocks _uniqueBlocks;

        public Blocks()
        {
            _blocks       = new int?[SIZE, SIZE];
            _uniqueBlocks = new UniqueBlocks();
            Override      = true;
        }

        private bool IsPositionOccupied(MyPoint p)
        {
            if (_blocks[p.X, p.Y] != null)
            {
                if (!Override)
                    return true;

                Delete(p);
            }

            return false;
        }

        public BlockImage GetBlock(BlockSize size, int x, int y)
        {
            if(x < 0 || y < 0)
                return null;

            if(x >= SIZE || y >= SIZE)
                return null;

            var id = _blocks[x, y];

            if(id == null)
                return null;

            return BlockImages.GetImageBlock(size, id.Value);
        }


        public void Add(MyPoint p, int id)
        {
            if(p.X < 0 || p.Y < 0)
                return;

            if(p.X >= SIZE || p.Y >= SIZE)
                return;

            if (IsPositionOccupied(p))
                return;

            HandleStartBlock(p, id);
            _blocks[p.X, p.Y] = id;
        }

        private void HandleStartBlock(MyPoint p, int blockid)
        {
            foreach (var startBlock in _uniqueBlocks.GetBlocks())
            {
                if(blockid != startBlock.ID)
                    continue;

                if(startBlock.Position != null)
                    Delete(startBlock.Position.Value);

                startBlock.Position = p;
            }
        }

        public void Delete(MyPoint p)
        {
            _blocks[p.X, p.Y] = null;
        }

        public MyPoint? GetStartPosition()
        {
            return _uniqueBlocks?.Player1?.Position;
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

                    yield return new SimpleBlock(id.Value) { Position = new MyPoint(x, y) };
                }
            }


        }

    }
}
