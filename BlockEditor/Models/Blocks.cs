using LevelModel.Models;
using LevelModel.Models.Components;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BlockEditor.Models
{

    public class Blocks
    {

        public const int SIZE = 2_000;

        private ImageBlock[,] _blocks;

        public bool Override { get; set; }

        public int BlockWidth { get; set; }
        public int BlockHeight { get; set; }

        private readonly UniqueBlocks _uniqueBlocks;

        public Blocks()
        {
            _blocks       = new ImageBlock[SIZE, SIZE];
            _uniqueBlocks = new UniqueBlocks();
            
            Override    = true;
            BlockWidth  = 40;
            BlockHeight = 40;
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

        private ImageBlock CreateBlock(ImageBlock selectedBlock)
        {
            if (selectedBlock == null)
                return null;

            var block = new ImageBlock();
            var image = selectedBlock.Source as BitmapImage;

            if (image == null)
                return null;

            var scaleX   = BlockWidth / selectedBlock.Source.Width;
            var scaleY   = BlockHeight / selectedBlock.Source.Height;
            block.Source = new TransformedBitmap(image, new ScaleTransform(scaleX, scaleY));
            block.ID     = selectedBlock.ID;

            return block;
        }

        public ImageBlock Get(int x, int y)
        {
            if(x < 0 || y < 0)
                return null;

            if(x >= SIZE || y >= SIZE)
                return null;

            return _blocks[x, y];
        }


        public void Add(MyPoint p, ImageBlock selectedBlock)
        {
            if (selectedBlock == null)
                return;

            if(p.X < 0 || p.Y < 0)
                return;

            if(p.X >= SIZE || p.Y >= SIZE)
                return;

            if (IsPositionOccupied(p))
                return;

            var block = CreateBlock(selectedBlock);

            if (block == null)
                return;

            HandleStartBlock(p, block);
            _blocks[p.X, p.Y] = block;
        }

        private void HandleStartBlock(MyPoint p, ImageBlock block)
        {
            if(block == null)
                return;

            if(block.ID == 0)
                return;

            foreach (var startBlock in _uniqueBlocks.GetBlocks())
            {
                if(block.ID != startBlock.ID)
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
                    var block = _blocks[x, y];

                    if(block == null)
                        continue;

                    yield return new SimpleBlock(block.ID) { Position = new MyPoint(x, y) };
                }
            }


        }

    }
}
