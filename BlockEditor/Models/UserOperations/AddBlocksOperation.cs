using BlockEditor.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockEditor.Models
{
    public class AddBlocksOperation : IUserOperation
    {
        private readonly Map _map;
        private IEnumerable<SimpleBlock> _blocks;


        public AddBlocksOperation(Map map, IEnumerable<SimpleBlock> blocks)
        {
            _map = map;
            _blocks = blocks;
        }

        public bool Execute()
        {
            if (_blocks == null || !_blocks.Any())
                return false;

            var addedBlocks = new List<SimpleBlock>();

            try
            {
                foreach (var block in _blocks)
                {
                    if (block?.Position == null)
                        continue;

                    _map?.Blocks.Add(block.Position.Value, block.ID);
                    addedBlocks.Add(block);
                }
            }
            catch (Exception ex)
            {
                _blocks = addedBlocks;
                MessageUtil.ShowError(ex.Message);
            }

            return true;
        }

        public bool Undo()
        {
            if (_blocks == null)
                return false;

            var removedBlocks = new List<SimpleBlock>();
            
            try
            {
                foreach (var block in _blocks)
                {
                    if (block?.Position == null)
                        continue;

                    _map?.Blocks.Delete(block.Position.Value);
                }
            }
            catch (Exception ex)
            {
                _blocks = removedBlocks;
                MessageUtil.ShowError(ex.Message);
            }

            return true;
        }

    }
}
