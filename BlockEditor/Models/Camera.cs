using BlockEditor.Models;
using System.Windows.Input;

namespace BlockEditor.Models
{
    public struct Camera
    {
        public MyPoint Position { get; set; }

        public const int MOVE_STRENGTH = 20;

        public Camera(MyPoint p)
        {
            Position = p;
        }

        public Camera(int x, int y)
        {
            Position = new MyPoint(x, y);
        }

        public Camera Move(int blockWidth, int blockHeight)
        {
            var currentX = Position.X;
            var currentY = Position.Y;

            if (Keyboard.IsKeyDown(Key.Up))
                currentY -= MOVE_STRENGTH;

            if (Keyboard.IsKeyDown(Key.Down))
                currentY += MOVE_STRENGTH;

            if (Keyboard.IsKeyDown(Key.Right))
                currentX += MOVE_STRENGTH;

            if (Keyboard.IsKeyDown(Key.Left))
                currentX -= MOVE_STRENGTH;

            if (currentX < 0)
                currentX = 0;

            if (currentY < 0)
                currentY = 0;

            var maxWidth = Blocks.SIZE * blockWidth;
            if (currentX > maxWidth)
                currentX = maxWidth;

            var maxHeight = Blocks.SIZE * blockHeight;
            if (currentY > maxHeight)
                currentY = maxHeight;

            return new Camera(currentX, currentY);
        }
    }
}
