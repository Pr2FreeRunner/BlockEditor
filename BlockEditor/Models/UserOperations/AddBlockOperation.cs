using LevelModel.Models.Components;

namespace BlockEditor.Models
{
    public class AddBlockOperation : IUserOperation
    {
        private readonly Map _map;
        private readonly SimpleBlock _block;
        private SimpleBlock _oldBlock;
        private MyPoint? _oldStartPosition;

        public AddBlockOperation(Map map, SimpleBlock b)
        {
            _map = map;
            _block = b;
        }

        public bool Execute()
        {
            if(_map?.Blocks == null || _block.IsEmpty())
                return false;

            _oldBlock = _map.Blocks.GetBlock(_block.Position, false);

            if (!_oldBlock.IsEmpty() && _oldBlock.ID == _block.ID)
                return false;

            if (Block.IsStartBlock(_block.ID))
            {
                var oldStartBlock = _map.Blocks.GetBlock(_block.Position, true);

                if(oldStartBlock.ID == _block.ID)
                    return false; 

                _oldStartPosition = _map.Blocks.StartBlocks.GetPosition(_block.ID);
            }

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

            if(Block.IsStartBlock(_block.ID))
            {
                if(_oldStartPosition == null)
                    return false;

                _map.Blocks.Add(new SimpleBlock(_block.ID, _oldStartPosition.Value));
            }
           
            if(!_oldBlock.IsEmpty())
                _map.Blocks.Add(_oldBlock);

            return true;
        }
    }

}
