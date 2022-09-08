using System;
using System.Diagnostics;
using System.Windows.Input;

namespace BlockEditor.Helpers
{

    public class RelayCommand : ICommand
    {
        Action<object> _execute;
        readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute) : this(execute, null) { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }


        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter)
        {
            try
            {
                _execute(parameter);
            }
            catch(Exception ex)
            {
                MessageUtil.ShowError(ex.Message);
            }
        }


        public void RaiseCanExecuteChanged()
        {
            /// Force an update in CanExcute, call this on the GUI thread
            CommandManager.InvalidateRequerySuggested();
        }

    }
}
