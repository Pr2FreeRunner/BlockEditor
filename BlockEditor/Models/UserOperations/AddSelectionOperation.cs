using System;
using System.Collections.Generic;
using System.Text;

namespace BlockEditor.Models
{
    public class AddSelectionOperation : IUserOperation
    {
        private readonly Map _map;
        private readonly IEnumerable<SimpleBlock> _selection;


        public AddSelectionOperation(Map map, IEnumerable<SimpleBlock> selection)
        {
            _map = map;
            _selection = selection;
        }

        public void Execute()
        {
            if (_selection == null)
                return;

            foreach (var block in _selection)
            {
                if (block?.Position == null)
                    continue;

                _map?.Blocks.Add(block.Position.Value, block.ID);
            }
        }

        public void Undo()
        {
            if (_selection == null)
                return;

            foreach (var block in _selection)
            {
                if (block?.Position == null)
                    continue;

                _map?.Blocks.Delete(block.Position.Value);
            }
        }

    }
}
