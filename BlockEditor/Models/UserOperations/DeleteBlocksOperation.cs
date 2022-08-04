﻿using System;
using System.Collections.Generic;

namespace BlockEditor.Models
{
    public class DeleteBlocksOperation : IUserOperation
    {
        private readonly AddBlocksOperation _add;


        public DeleteBlocksOperation(Map map, IEnumerable<SimpleBlock> blocks)
        {
            _add = new AddBlocksOperation(map, blocks);
        }

        public bool Execute()
        {
            return _add.Undo();
        }

        public bool Undo()
        {
           return  _add.Execute();
        }

    }
}
