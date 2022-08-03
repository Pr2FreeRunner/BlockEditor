using BlockEditor.Utils;
using BlockEditor.ViewModels;
using System.Collections.Generic;
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
                return  null;;

            var x = p.Value.X + Camera.Position.X;
            var y = p.Value.Y + Camera.Position.Y;

            var pos = new MyPoint(x, y);
            return Map.GetMapIndex(pos);
        }

        public void AddBlock(MyPoint? p, int? id, bool isMapIndex = false)
        {
            var index = isMapIndex ? p : GetMapIndex(p);

            if (index == null || id == null || Map == null)
                return;

            if (Map.Blocks.GetBlockId(index.Value.X, index.Value.Y) == id.Value)
                return;

            var op = new AddBlockOperation(Map, id.Value, index.Value);
            UserOperations.Execute(op);
        }

        internal void AddBlocks(MyPoint? p, int?[,] selectedBlocks)
        {
            var index = GetMapIndex(p);

            if (index == null || selectedBlocks == null || Map == null)
                return;

            var width = selectedBlocks.GetLength(0);
            var height  = selectedBlocks.GetLength(1);
            var blocks  = new List<SimpleBlock>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var id         = selectedBlocks[x, y];
                    var blockIndex = new MyPoint(index.Value.X + x - width  + 1, index.Value.Y + y - height + 1);
                    var currentID  = Map.Blocks.GetBlockId(blockIndex.X, blockIndex.Y);
                     
                    if(currentID == id)
                        continue;

                    if(id == null)
                        continue;

                    blocks.Add(new SimpleBlock(id.Value, blockIndex));
                }
            }

            if(!blocks.Any())
                return;

            var op = new AddSelectionOperation(Map, blocks);
            UserOperations.Execute(op);
        }

        public void DeleteBlock(MyPoint? p)
        {
            var index = GetMapIndex(p);

            if (index == null || Map == null)
                return;

            var blockId = Map.Blocks.GetBlockId(index.Value.X, index.Value.Y);

            if (blockId == null)
                return;

            var op = new DeleteBlockOperation(Map, blockId.Value, index.Value);
            UserOperations.Execute(op);
        }

        public void GoToStartPosition()
        {
            var p = Map.Blocks.GetStartPosition();

            if (p == null)
                return;

            var size = Map.BlockSize.GetPixelSize();
            var x = p.Value.X * size - (GameImage.Width / 2);
            var y = p.Value.Y * size - (GameImage.Height / 2);

            Camera.Position = new MyPoint(x, y); ;
        }

    }
}
