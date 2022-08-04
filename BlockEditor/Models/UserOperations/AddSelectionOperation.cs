using BlockEditor.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockEditor.Models
{
    public class AddSelectionOperation : IUserOperation
    {
        private readonly Map _map;
        private IEnumerable<SimpleBlock> _selection;


        public AddSelectionOperation(Map map, IEnumerable<SimpleBlock> selection)
        {
            _map = map;
            _selection = selection;
        }

        public void Execute()
        {
            if (_selection == null)
                return;

            var addedBlocks = new List<SimpleBlock>();

            try
            {
                foreach (var block in _selection)
                {
                    if (block?.Position == null)
                        continue;

                    _map?.Blocks.Add(block.Position.Value, block.ID);
                    addedBlocks.Add(block);
                }
            }
            catch (Exception ex)
            {
                _selection = addedBlocks;
                MessageUtil.ShowError(ex.Message);
            }
        }

        public void Undo()
        {
            if (_selection == null)
                return;

            var removedBlocks = new List<SimpleBlock>();
            
            try
            {
                foreach (var block in _selection)
                {
                    if (block?.Position == null)
                        continue;

                    _map?.Blocks.Delete(block.Position.Value);
                }
            }
            catch (Exception ex)
            {
                _selection = removedBlocks;
                MessageUtil.ShowError(ex.Message);
            }
        }

    }
}
