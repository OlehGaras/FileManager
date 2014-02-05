using System;
using System.Windows.Input;

namespace FileManager
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> mExecute;
        private readonly Predicate<object> mCanExecute;


        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            mExecute = execute;
            mCanExecute = canExecute;
        }


        public bool CanExecute(object parameter)
        {
            return mCanExecute == null || mCanExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            mExecute(parameter);
        }
    }
}
