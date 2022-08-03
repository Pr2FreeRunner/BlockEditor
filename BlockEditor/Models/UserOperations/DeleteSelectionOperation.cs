using System;
using System.Collections.Generic;

namespace BlockEditor.Models
{
    public class DeleteSelectionOperation : IUserOperation
    {
        private readonly AddSelectionOperation _add;


        public DeleteSelectionOperation(Map map, IEnumerable<SimpleBlock> blocks)
        {
            _add = new AddSelectionOperation(map, blocks);
        }

        public void Execute()
        {
            _add?.Undo();
        }

        public void Undo()
        {
            _add?.Execute();
        }

    }
}
