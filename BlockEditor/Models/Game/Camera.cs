using BlockEditor.Utils;
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
            set { lock (_lock) { _position = value; } }
        }

        public MyPoint ScreenSize { get; internal set; }

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
            var moveStrength = MOVE_STRENGTH;

            var ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            var alt  = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);

            if(alt)
                moveStrength *= 3;

            if (Keyboard.IsKeyDown(Key.Up) || !ctrl && Keyboard.IsKeyDown(Key.W))
                currentY -= moveStrength;

            if (Keyboard.IsKeyDown(Key.Down) || !ctrl && Keyboard.IsKeyDown(Key.S))
                currentY += moveStrength;

            if (Keyboard.IsKeyDown(Key.Right) || !ctrl && Keyboard.IsKeyDown(Key.D))
                currentX += moveStrength;

            if (Keyboard.IsKeyDown(Key.Left) || !ctrl && Keyboard.IsKeyDown(Key.A))
                currentX -= moveStrength;

            if (currentX < 0)
                currentX = 0;

            if (currentY < 0)
                currentY = 0;

            var maxWidth = Blocks.SIZE * blockSize - ScreenSize.X;
            var maxHeight = Blocks.SIZE * blockSize - ScreenSize.Y;

            if (currentX > maxWidth)
                currentX = maxWidth;

            if (currentY > maxHeight)
                currentY = maxHeight;

            Position = new MyPoint(currentX, currentY);
        }
    }
}
