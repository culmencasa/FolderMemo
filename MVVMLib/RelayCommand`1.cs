using System;
using System.Windows.Input;

namespace MVVMLib
{
    /// <summary>
    /// 有参Command
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelayCommand<T> : CommandBase
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? new Func<T, bool>(_ => true);
        }

        public override bool CanExecute(object parameter)
        {
            return _canExecute((T)parameter);
        }

        public override void Execute(object parameter)
        {
            var typedParameter = (T)parameter;
            if (_canExecute(typedParameter))
            {
                _execute(typedParameter);
            }
        }
    }
}