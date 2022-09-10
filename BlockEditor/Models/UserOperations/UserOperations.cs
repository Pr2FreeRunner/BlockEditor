using BlockEditor.Helpers;
using BlockEditor.Models;
using System;
using System.Collections.Generic;

namespace BlockEditor.ViewModels
{

    public class UserOperations
    {
        private readonly Stack<IUserOperation> _operations;
        private readonly Stack<IUserOperation> _undos;
        private readonly object _lock = new object();

        public RelayCommand UndoCommand { get; }
        public RelayCommand RedoCommand { get; }

        public AddBlocksOperation LastAddBlocksOperation { get; set; }


        public UserOperations()
        {
            _operations = new Stack<IUserOperation>();
            _undos = new Stack<IUserOperation>();

            UndoCommand = new RelayCommand((_) => Undo(), (_) => CanUndo());
            RedoCommand = new RelayCommand((_) => Redo(), (_) => CanRedo());
        }

        public bool CanUndo()
        {
            return _operations.Count > 0;
        }

        public bool CanRedo()
        {
            return _undos.Count > 0;
        }

        public void Execute(IUserOperation op)
        {
            if(op == null)
                return;

            try
            {
                lock (_lock)
                {
                    _undos.Clear();

                    var success = op.Execute();

                    if(success)
                        _operations.Push(op);

                    AddLastAddBlocksOperation(success, op);
                }
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        private void AddLastAddBlocksOperation(bool success, IUserOperation op)
        {
            if (success && op is AddBlocksOperation last)
                LastAddBlocksOperation = last;
            else
                LastAddBlocksOperation = null;
        }

        private void UndoLastAddBlocksOperation(bool success, IUserOperation op)
        {
            if(!success)
                return;

            if(LastAddBlocksOperation == null)
                return;

            if (!(op is AddBlocksOperation last))
                return;

            if(last.GetID() == LastAddBlocksOperation.GetID())
                LastAddBlocksOperation = null;
        }


        public void Undo()
        {
            try
            {
                if (!CanUndo())
                    return;

                lock (_lock)
                {
                    var op = _operations.Pop();
                    var success = op.Undo();

                    if (success)
                        _undos.Push(op);

                    UndoLastAddBlocksOperation(success, op);
                }
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        public void Redo()
        {
            try
            {
                if (!CanRedo())
                    return;

                lock(_lock)
                {
                    var op = _undos.Pop();
                    var success = op.Execute(true);

                    if(success)
                        _operations.Push(op);

                    AddLastAddBlocksOperation(success, op);
                }
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }

        internal void Clear()
        {
            lock (_lock)
            {
                _operations.Clear();
                _undos.Clear();
            }

        }
    }
}
