using BlockEditor.Utils;
using BlockEditor.ViewModels;

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

            var height = selectedBlocks.GetLength(0);
            var width  = selectedBlocks.GetLength(1);

            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    var blockIndex = new MyPoint(index.Value.X + column - height, index.Value.Y + row - width);
                    var id = selectedBlocks[row, column];

                    if(id == null)
                        continue;

                    AddBlock(blockIndex, id, true);
                }
            }
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
