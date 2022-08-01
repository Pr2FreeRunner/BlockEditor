using BlockEditor.Helpers;
using System;
using System.Collections.Generic;

namespace BlockEditor.Models.UserOperations
{

    public class UserOperations
    {
        private readonly Stack<IUserOperation> _operations;
        private readonly Stack<IUserOperation> _undos;
        private readonly object _lock = new object();

        public UserOperations()
        {
            _operations = new Stack<IUserOperation>();
            _undos = new Stack<IUserOperation>();
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
            try
            {
                lock (_lock)
                {
                    _undos.Clear();
                    op.Execute();
                    _operations.Push(op);
                }
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError("Error occurred when executing user operation..."
                                + Environment.NewLine + Environment.NewLine + ex.Message);
            }
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
                    op.Undo();
                    _undos.Push(op);
                }
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError("Error occurred when undoing user operation..."
                                + Environment.NewLine + Environment.NewLine + ex.Message);
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
                    op.Execute();
                    _operations.Push(op);
                }
            }
            catch (Exception ex)
            {
                MessageUtil.ShowError("Error occurred when redoing user operation..."
                     + Environment.NewLine + Environment.NewLine + ex.Message);
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
