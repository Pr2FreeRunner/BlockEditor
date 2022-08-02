using BlockEditor.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace BlockEditor.Models
{
   public class Game
    {

        public GameImage GameImage { get; set; }

        public GameEngine Engine { get; }

        public Camera Camera { get; set; }

        public Map Map { get; set; }

        public Game()
        {
            Map = new Map();
            Engine = new GameEngine();
            Camera = new Camera();
            GameImage = new GameImage(0, 0);
        }


        public IUserOperation AddBlock(Point? p, int? id)
        {
            if (p == null || id == null || Map == null)
                return null;

            var x = p.Value.X + Camera.Position.X;
            var y = p.Value.Y + Camera.Position.Y;

            var pos = new Point(x, y);
            var index = Map.GetMapIndex(pos);

            if (Map.Blocks.GetBlockId(index.X, index.Y) == id.Value)
                return null;

            return new AddBlockOperation(Map, id.Value, index);
        }

        public IUserOperation DeleteBlock(Point? p)
        {
            if (p == null || Map == null)
                return null;

            var x = p.Value.X + Camera.Position.X;
            var y = p.Value.Y + Camera.Position.Y;

            var index   = Map.GetMapIndex(new Point(x, y));
            var blockId = Map.Blocks.GetBlockId(index.X, index.Y);

            if (blockId == null)
                return null;

            return new DeleteBlockOperation(Map, blockId.Value, index);
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
