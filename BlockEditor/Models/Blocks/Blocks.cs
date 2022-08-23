using BlockEditor.Utils;
using LevelModel.Models.Components;
using System;
using System.Collections.Generic;

namespace BlockEditor.Models
{
    public class Blocks
    {

        public const int SIZE = 2_000;
        public const int LIMIT = 50_000;

        private SimpleBlock[,] _blocks;

        public bool Overwrite { get; set; }
        public int BlockCount;

        public UniqueBlocks StartBlocks { get; }


        public Blocks()
        {
            _blocks = new SimpleBlock[SIZE, SIZE];
            StartBlocks = new UniqueBlocks();
            Overwrite = true;
        }


        private bool IsPositionOccupied(SimpleBlock b)
        {
            if (b.IsEmpty())
                return false;

            var x = b.Position.Value.X;
            var y = b.Position.Value.Y;

            if (!_blocks[x, y].IsEmpty())
            {
                if (!Overwrite)
                    return true;

                Delete(b);
            }

            return false;
        }

        public SimpleBlock GetBlock(int x, int y, bool startBlocks = true)
        {
            if (x < 0 || y < 0)
                return SimpleBlock.None;

            if (x >= SIZE || y >= SIZE)
                return SimpleBlock.None;

            var block = StartBlocks.GetBlock(new MyPoint(x, y));

            if (!block.IsEmpty() && startBlocks)
                return block;

            return _blocks[x, y];
        }

        public SimpleBlock GetBlock(MyPoint? point, bool startBlocks = true)
        {
            if (point == null)
                return new SimpleBlock();

            return GetBlock(point.Value.X, point.Value.Y, startBlocks);
        }


        public void Add(SimpleBlock block)
        {
            if (block.Position == null)
                return;

            if (block.Position.Value.X < 0 || block.Position.Value.Y < 0)
                return;

            if (block.Position.Value.X >= SIZE || block.Position.Value.Y >= (SIZE - 1))
                return;

            if (Block.IsStartBlock(block.ID))
            {
                AddStartBlock(block);
            }
            else
            {
                if (!IsPositionOccupied(block))
                {
                    if (BlockCount >= LIMIT)
                        throw new BlockLimitException();

                    if(block.ID == Block.TELEPORT)
                        BlockImages.AddTeleportBlock(block.Options);

                    _blocks[block.Position.Value.X, block.Position.Value.Y] = block;
                    BlockCount++;
                }
            }
        }

        private void AddStartBlock(SimpleBlock b)
        {
            if (b.IsEmpty())
                return;

            void IncreaseBlockCount(SimpleBlock startBlock)
            {
                if (startBlock.IsEmpty())
                {
                    if (BlockCount >= LIMIT)
                        throw new BlockLimitException();

                    BlockCount++;
                }
            }

            var nr = b.ID - Block.START_BLOCK_P1 + 1;

            if (nr == 1)
            {
                IncreaseBlockCount(StartBlocks.Player1);
                StartBlocks.Player1 = b;
            }
            else if (nr == 2)
            {
                IncreaseBlockCount(StartBlocks.Player2);
                StartBlocks.Player2 = b;
            }
            else if (nr == 3)
            {
                IncreaseBlockCount(StartBlocks.Player3);
                StartBlocks.Player3 = b;
            }
            else if (nr == 4)
            {
                IncreaseBlockCount(StartBlocks.Player4);
                StartBlocks.Player4 = b;
            }
        }

        public void Delete(SimpleBlock block)
        {
            if (block.Position == null)
                return;

            var x = block.Position.Value.X;
            var y = block.Position.Value.Y;

            if (!GetBlock(x, y).IsEmpty())
                BlockCount--;

            if(Block.IsStartBlock(block.ID))
                StartBlocks.Remove(block.ID);
            else
                _blocks[x, y] = new SimpleBlock();
        }

        public IEnumerable<SimpleBlock> GetBlocks(bool startBlocks = false)
        {
            if (_blocks == null)
                yield break;

            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    var block = _blocks[x, y];

                    if (block.IsEmpty())
                        continue;

                    yield return block;
                }
            }

            if (!startBlocks)
                yield break;

            foreach (var start in StartBlocks.GetBlocks())
            {
                if (start.IsEmpty())
                    continue;

                yield return start;
            }

        }

        public void VerticalFlip()
        {
            _blocks = ArrayUtil.VerticalFlip(_blocks);

            // update the blocks internal positon
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    var b = _blocks[i, j];

                    if (b.IsEmpty())
                        continue;

                    _blocks[i, j] = new SimpleBlock(b.ID, i, j);
                }
            }

            if (!StartBlocks.Player1.IsEmpty())
            {
                var x = StartBlocks.Player1.Position.Value.X;
                var y = StartBlocks.Player1.Position.Value.Y;

                StartBlocks.Player1 = new SimpleBlock(Block.START_BLOCK_P1, x, SIZE - y - 1);
            }

            if (!StartBlocks.Player2.IsEmpty())
            {
                var x = StartBlocks.Player2.Position.Value.X;
                var y = StartBlocks.Player2.Position.Value.Y;

                StartBlocks.Player2 = new SimpleBlock(Block.START_BLOCK_P2, x, SIZE - y - 1);
            }

            if (!StartBlocks.Player3.IsEmpty())
            {
                var x = StartBlocks.Player3.Position.Value.X;
                var y = StartBlocks.Player3.Position.Value.Y;

                StartBlocks.Player3 = new SimpleBlock(Block.START_BLOCK_P3, x, SIZE - y - 1);
            }

            if (!StartBlocks.Player4.IsEmpty())
            {
                var x = StartBlocks.Player4.Position.Value.X;
                var y = StartBlocks.Player4.Position.Value.Y;

                StartBlocks.Player4 = new SimpleBlock(Block.START_BLOCK_P4, x, SIZE - y - 1);
            }
        }

        public void HorizontalFlip()
        {
            _blocks = ArrayUtil.HorizontalFlip(_blocks);

            // update the blocks internal positon
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    var b = _blocks[i, j];

                    if(b.IsEmpty())
                        continue;

                    _blocks[i, j] = new SimpleBlock(b.ID, i, j);
                }
            }

            if (!StartBlocks.Player1.IsEmpty())
            {
                var x = StartBlocks.Player1.Position.Value.X;
                var y = StartBlocks.Player1.Position.Value.Y;

                StartBlocks.Player1 = new SimpleBlock(Block.START_BLOCK_P1, SIZE - x - 1, y);
            }

            if (!StartBlocks.Player2.IsEmpty())
            {
                var x = StartBlocks.Player2.Position.Value.X;
                var y = StartBlocks.Player2.Position.Value.Y;

                StartBlocks.Player2 = new SimpleBlock(Block.START_BLOCK_P2, SIZE - x -1, y);
            }

            if (!StartBlocks.Player3.IsEmpty())
            {
                var x = StartBlocks.Player3.Position.Value.X;
                var y = StartBlocks.Player3.Position.Value.Y;

                StartBlocks.Player3 = new SimpleBlock(Block.START_BLOCK_P3, SIZE - x - 1, y);
            }

            if (!StartBlocks.Player4.IsEmpty())
            {
                var x = StartBlocks.Player4.Position.Value.X;
                var y = StartBlocks.Player4.Position.Value.Y;

                StartBlocks.Player4 = new SimpleBlock(Block.START_BLOCK_P4, SIZE - x - 1, y);
            }
        }

        public void Rotate()
        {
            _blocks = ArrayUtil.RotateRight(_blocks);

            // update the blocks internal positon
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    var b = _blocks[i, j];

                    if (b.IsEmpty())
                        continue;

                    _blocks[i, j] = new SimpleBlock(b.ID, i, j);
                }
            }

            if (!StartBlocks.Player1.IsEmpty())
            {
                var x = StartBlocks.Player1.Position.Value.X;
                var y = StartBlocks.Player1.Position.Value.Y;

                StartBlocks.Player1 = new SimpleBlock(Block.START_BLOCK_P1, y, SIZE - x - 1);
            }

            if (!StartBlocks.Player2.IsEmpty())
            {
                var x = StartBlocks.Player2.Position.Value.X;
                var y = StartBlocks.Player2.Position.Value.Y;

                StartBlocks.Player2 = new SimpleBlock(Block.START_BLOCK_P2, y, SIZE - x - 1);
            }

            if (!StartBlocks.Player3.IsEmpty())
            {
                var x = StartBlocks.Player3.Position.Value.X;
                var y = StartBlocks.Player3.Position.Value.Y;

                StartBlocks.Player3 = new SimpleBlock(Block.START_BLOCK_P3, y, SIZE - x - 1);
            }

            if (!StartBlocks.Player4.IsEmpty())
            {
                var x = StartBlocks.Player4.Position.Value.X;
                var y = StartBlocks.Player4.Position.Value.Y;

                StartBlocks.Player4 = new SimpleBlock(Block.START_BLOCK_P4, y, SIZE - x - 1);
            }

        }

    }
}
