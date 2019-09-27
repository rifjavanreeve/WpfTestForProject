using System;
using System.Windows.Input;

namespace TestForJPsProject
{
    //using the same .cs file for the DelegateCommand of a generic type and the typeless DelegateCommand
    public class DelegateCommand<T> : ICommand
    {
        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        public DelegateCommand(Action<T> execute) 
            : this(execute, null) { }

        public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute((parameter == null) 
                ? default 
                : (T)Convert.ChangeType(parameter, typeof(T)));
        }

        public void Execute(object parameter)
        {
            _execute((parameter == null) 
                ? default 
                : (T)Convert.ChangeType(parameter, typeof(T)));
        }

        //we can use the RaiseCanExecuteChanged method to invoke the CanExecuteChanged event
        //which changes the CanExecute state of a command
        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    //the DelegateCommand which is not of a generic type
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        //added a constructor with an action without a generic type as parameter
        public DelegateCommand(Action execute)
            :this(execute, null)
        {
        }

        //overloaded constructor with action w/o generic type to accept a canExecute predicate
        public DelegateCommand(Action execute, 
            Predicate<object> canExecute)
        {
            //setting the private field _execute of type Action<object>
            //with the execute parameter of type Action w/o generic type
            _execute = (o) => execute();
            _canExecute = canExecute;
        }
        
        public DelegateCommand(Action<object> execute)
                       : this(execute, null)
        {
        }

        public DelegateCommand(Action<object> execute,
                       Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }
        
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
