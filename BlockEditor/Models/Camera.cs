using BlockEditor.Models;
using System.Windows.Input;
using static BlockEditor.Models.BlockImages;

namespace BlockEditor.Models
{
    public class Camera
    {

        private object _lock = new object();

        private MyPoint _position;

        public MyPoint Position
        {
            get { return _position; }
            set { lock (_lock)  { _position = value; } }
        }

        public const int MOVE_STRENGTH = 30;


        public Camera() { }

        public Camera(MyPoint p)
        {
            Position = p;
        }

        public Camera(int x, int y)
        {
            Position = new MyPoint(x, y);
        }


        public void Move(BlockSize size)
        {
            var currentX  = Position.X;
            var currentY  = Position.Y;
            var blockSize = size.GetPixelSize();

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

            var maxWidth  = Blocks.SIZE * blockSize;
            var maxHeight = Blocks.SIZE * blockSize;

            if (currentX > maxWidth)
                currentX = maxWidth;

            if (currentY > maxHeight)
                currentY = maxHeight;

            Position = new MyPoint(currentX, currentY);
        }
    }
}
