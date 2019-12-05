using System;
using System.Windows.Input;

namespace MVVMLib
{
    /// <summary>
    /// 无参Command
    /// </summary>
    public class RelayCommand : CommandBase
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;


        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? new Func<bool>(() => true);
        }


        public override bool CanExecute(object parameter)
        {
            return _canExecute();
        }


        public override void Execute(object parameter)
        {
            if (_canExecute())
            {
                _execute();
            }
        }
    }
}