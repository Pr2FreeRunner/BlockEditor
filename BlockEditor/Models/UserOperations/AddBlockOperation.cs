using BlockEditor.Models;

namespace BlockEditor.Models
{
    public class AddBlockOperation : IUserOperation
    {
        private readonly Map _map;
        private readonly MyPoint _point;
        private readonly int _blockID;


        public AddBlockOperation(Map map, int blockID, MyPoint p)
        {
            _map = map;
            _point = p;
            _blockID = blockID;
        }

        public void Execute()
        {
            _map?.Blocks.Add(_point, _blockID);
        }

        public void Undo()
        {
            _map?.Blocks.Delete(_point);
        }
    }
}
