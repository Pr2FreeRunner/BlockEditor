using BlockEditor.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockEditor.Models
{
    public class AddBlocksOperation : BaseOperation, IUserOperation
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

        public bool Execute(bool redo = false)
        {
            if (!_blocks.AnyBlocks())
                return false;

            var added = new List<AddBlockOperation>();

            try
            {
                if (_operations == null || !_operations.Any())
                    ExecuteBlocks(added, false, redo);
                else
                    ExecuteOperations(added, false, redo);
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }

            _operations = added;
            return _operations.Count != 0;
        }

        public bool Undo()
        {
            if (_blocks == null)
                return false;

            var removed = new List<AddBlockOperation>();

            using(new TempOverwrite(_map.Blocks, true))
            {
                try
                {
                    if (_operations == null || !_operations.Any())
                        ExecuteBlocks(removed, true);
                    else
                        ExecuteOperations(removed, true);
                }
                catch (Exception ex)
                {
                    MessageUtil.ShowError(ex.Message);
                }
            }

            _operations = removed;
            return true;
        }

        public IEnumerable<SimpleBlock> GetBlocks()
        {
            return _blocks;
        }

        private void ExecuteBlocks(List<AddBlockOperation> operations, bool undo, bool redo = false)
        {
            foreach (var block in _blocks)
            {
                if (block.IsEmpty())
                    continue;

                var op = new AddBlockOperation(_map, block);
                var ok = undo ? op.Undo() : op.Execute(redo);

                if (ok)
                    operations.Add(op);
            }
        }

        private void ExecuteOperations(List<AddBlockOperation> operations, bool undo, bool redo = false)
        {
            foreach (var op in _operations)
            {
                var ok = undo ? op.Undo() : op.Execute(redo);

                if (ok)
                    operations.Add(op);
            }
        }
    }
}
