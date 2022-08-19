using BlockEditor.Helpers;
using BlockEditor.Utils;
using BlockEditor.ViewModels;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BlockEditor.Models
{
   public class Game
    {

        public GameImage GameImage { get; set; }

        public GameEngine Engine { get; }

        public Camera Camera { get; set; }

        public Map Map { get; set; }

        public UserOperationsViewModel UserOperations { get; }


        public Game()
        {
            Map = new Map();
            Engine = new GameEngine();
            Camera = new Camera();
            GameImage = new GameImage(0, 0);
            UserOperations = new UserOperationsViewModel();
        }

        public MyPoint? GetMapIndex(MyPoint? p)
        {
            if (p == null || Map == null)
                return  null;

            var x = p.Value.X + Camera.Position.X;
            var y = p.Value.Y + Camera.Position.Y;

            var pos = new MyPoint(x, y);
            return Map.GetMapIndex(pos);
        }

        public void AddBlock(MyPoint? index, int? id, bool isMapIndex = false)
        {
            if (index == null || id == null || Map == null)
                return;

            var op = new AddBlockOperation(Map, new SimpleBlock(id.Value, index.Value));
            UserOperations.Execute(op);
        }

        internal void AddBlocks(IEnumerable<SimpleBlock> blocks)
        {
            if (blocks == null || Map == null)
                return;

            if (!blocks.Any())
                return;

            var op = new AddBlocksOperation(Map, blocks);
            UserOperations.Execute(op);
        }
 
        internal void AddSelection(MyPoint? index, int?[,] selectedBlocks)
        {
            if (index == null || selectedBlocks == null || Map == null)
                return;

            var width = selectedBlocks.GetLength(0);
            var height  = selectedBlocks.GetLength(1);
            var blocks  = new List<SimpleBlock>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var id = selectedBlocks[x, y];
                    var blockIndex = new MyPoint(index.Value.X + x - width  + 1, index.Value.Y + y - height + 1);
                    var currentBlock = Map.Blocks.GetBlock(blockIndex.X, blockIndex.Y);
                     
                    if(!currentBlock.IsEmpty() && currentBlock.ID == id)
                        continue;

                    if(id == null)
                        continue;

                    blocks.Add(new SimpleBlock(id.Value, blockIndex));
                }
            }

            if(!blocks.Any())
                return;

            var op = new AddBlocksOperation(Map, blocks);
            UserOperations.Execute(op);
        }

        public void DeleteSelection(MyRegion region)
        {
            if (region == null || !region.IsComplete() || Map == null)
                return;

            var blocks = new List<SimpleBlock>();

            for (int x = region.Start.Value.X; x < region.End.Value.X; x++)
            {
                for (int y = region.Start.Value.Y; y < region.End.Value.Y; y++)
                {
                    var block = Map.Blocks.GetBlock(x, y);

                    if (block.IsEmpty())
                        continue;

                    blocks.Add(block);
                }
            }

            if (!blocks.Any())
                return;

            var op = new DeleteBlocksOperation(Map, blocks);
            UserOperations.Execute(op);
        }

        public void DeleteBlock(MyPoint? index)
        {
            if (index == null || Map == null)
                return;

            var block = Map.Blocks.GetBlock(index.Value.X, index.Value.Y);

            if (block.IsEmpty())
                return;

            var op = new DeleteBlockOperation(Map, block);
            UserOperations.Execute(op);
        }

        public void GoToStartPosition()
        {
            var p = Map.Blocks.GetStartPosition();

            if (p == null)
            {
                MessageUtil.ShowWarning("The map contains no start position.");
                return;
            }

            var size = Map.BlockSize.GetPixelSize();
            var x = p.Value.X * size - (GameImage.Width / 2);
            var y = p.Value.Y * size - (GameImage.Height / 2);

            Camera.Position = new MyPoint(x, y);
        }

    }
}
