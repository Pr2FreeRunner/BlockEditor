using BlockEditor.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockEditor.Models
{
    public class AddBlocksOperation : IUserOperation
    {
        private readonly Map _map;
        private readonly IEnumerable<SimpleBlock> _blocks;
        private List<AddBlockOperation> _operations;

        public AddBlocksOperation(Map map, IEnumerable<SimpleBlock> blocks)
        {
            _map = map;
            _blocks = blocks;
            _operations = new List<AddBlockOperation>();
        }

        public bool Execute()
        {
            if (_blocks == null || !_blocks.Any())
                return false;

            var added = new List<AddBlockOperation>();

            try
            {
                if (_operations == null || !_operations.Any())
                    ExecuteBlocks(added, false);
                else
                    ExecuteOperations(added, false);
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }

            _operations = added;
            return true;
        }

        public bool Undo()
        {
            if (_blocks == null)
                return false;

            var removed = new List<AddBlockOperation>();

            try
            {
                if(_operations == null || !_operations.Any())
                    ExecuteBlocks(removed, true);
                else
                    ExecuteOperations(removed, true);
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }

            _operations = removed;
            return true;
        }

        private void ExecuteBlocks(List<AddBlockOperation> operations, bool undo)
        {
            foreach (var block in _blocks)
            {
                if (block?.Position == null)
                    continue;

                var op = new AddBlockOperation(_map, block.ID, block.Position.Value);
                var ok = undo ? op.Undo() : op.Execute();

                if (ok)
                    operations.Add(op);
            }
        }

        private void ExecuteOperations(List<AddBlockOperation> operations, bool undo)
        {
            foreach (var op in _operations)
            {
                var ok = undo ? op.Undo() : op.Execute();

                if (ok)
                    operations.Add(op);
            }
        }
    }
}
