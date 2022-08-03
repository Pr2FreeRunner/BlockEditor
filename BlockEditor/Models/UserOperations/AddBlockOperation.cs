using BlockEditor.Models;
using LevelModel.Models.Components;

namespace BlockEditor.Models
{
    public class AddBlockOperation : IUserOperation
    {
        private readonly Map _map;
        private readonly MyPoint _point;
        private readonly int _blockID;
        private MyPoint? _oldPosition;

        public AddBlockOperation(Map map, int blockID, MyPoint p)
        {
            _map = map;
            _point = p;
            _blockID = blockID;
        }

        public void Execute()
        {
            if(_map?.Blocks == null)
                return;

            if (Block.IsStartBlock(_blockID))
                _oldPosition = _map.Blocks.StartBlocks.GetPosition(_blockID);
            
            _map?.Blocks.Add(_point, _blockID);
        }

        public void Undo()
        {
            if (_map?.Blocks == null)
                return;

            if (Block.IsStartBlock(_blockID))
            {
                if(_oldPosition == null)
                    return;

                _map.Blocks.Add(_oldPosition.Value, _blockID); 
            }
            else
            { 
                _map.Blocks.Delete(_point);
            }
        }
    }

}
