using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PokeDex.WpfApp.ViewModel.Commands
{
    public class AsyncCommand : ICommand
    {
        private Func<Task> _action;

        public AsyncCommand(Func<Task> action)
        {
            _action = action;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _action.Invoke();
        }
    }

    public class AsyncCommand<T> : ICommand where T : class
    {
        private Func<T, Task> _action;

        public AsyncCommand(Func<T, Task> action)
        {
            _action = action;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _action.Invoke(parameter as T);
        }
    }
}
