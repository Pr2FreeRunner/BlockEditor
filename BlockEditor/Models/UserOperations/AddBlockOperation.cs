using LevelModel.Models.Components;
using static System.Windows.Forms.Design.AxImporter;
using System;
using BlockEditor.Helpers;

namespace BlockEditor.Models
{
    public class AddBlockOperation : BaseOperation, IUserOperation
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

        public bool Execute(bool redo = false)
        {
            if (_map?.Blocks == null || _block.IsEmpty())
                return false;

            using (new TempOverwrite(_map.Blocks, true, redo))
            {
                _oldBlock = _map.Blocks.GetBlock(_block.Position, false);

                if (!_oldBlock.IsEmpty() && _oldBlock.ID == _block.ID && string.Equals(_oldBlock.Options, _block.Options, StringComparison.InvariantCultureIgnoreCase))
                    return false;

                if (Block.IsStartBlock(_block.ID))
                {
                    _oldBlock = SimpleBlock.None;
                    var oldStartBlock = _map.Blocks.GetBlock(_block.Position, true);

                    if (oldStartBlock.ID == _block.ID)
                        return false;

                    _oldStartPosition = _map.Blocks.StartBlocks.GetPosition(_block.ID);
                }
                else
                {
                    if (!_oldBlock.IsEmpty() && !_map.Blocks.Overwrite)
                        return false;
                }

                _map?.Blocks.Add(_block);

                return true;
            }
        }

        public bool Undo()
            {
            if (_map?.Blocks == null)
                return false;

            if (_block.IsEmpty())
                return false;

            using (new TempOverwrite(_map.Blocks, true))
            {
                _map.Blocks.Delete(_block);

                if (Block.IsStartBlock(_block.ID))
                {
                    if (_oldStartPosition == null)
                        _oldStartPosition = _block.Position;
                    else
                        _map.Blocks.Add(new SimpleBlock(_block.ID, _oldStartPosition.Value));
                }

                if (!_oldBlock.IsEmpty())
                    _map.Blocks.Add(_oldBlock);

            }

            return true;
        }
    }

}
