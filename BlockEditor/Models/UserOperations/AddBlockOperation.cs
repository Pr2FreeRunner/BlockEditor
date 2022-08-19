using LevelModel.Models.Components;

namespace BlockEditor.Models
{
    public class AddBlockOperation : IUserOperation
    {
        private readonly Map _map;
        private readonly SimpleBlock _block;
        private SimpleBlock _oldBlock;


        public AddBlockOperation(Map map, SimpleBlock b)
        {
            _map = map;
            _block = b;
        }

        public bool Execute()
        {
            if(_map?.Blocks == null || _block.IsEmpty())
                return false;

            _oldBlock = _map.Blocks.GetBlock(_block.Position);

            if (!_oldBlock.IsEmpty() && _oldBlock.ID == _block.ID)
                return false;

            _map?.Blocks.Add(_block);

            return true;
        }

        public bool Undo()
        {
            if (_map?.Blocks == null)
                return false;

            if (_block.IsEmpty())
                return false;

            _map.Blocks.Delete(_block);
           
            if(!_oldBlock.IsEmpty())
                _map.Blocks.Add(_oldBlock);

            return true;
        }
    }

}
