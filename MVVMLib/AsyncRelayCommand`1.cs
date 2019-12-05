using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMLib
{
    /// <summary>
    /// 异步有参Command
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncRelayCommand<T> : CommandBase
    {
        #region 字段

        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;
        private bool _isExecuting;

        #endregion

        #region 属性

        /// <summary>
        /// 用于控制Command的执行，避免重复执行
        /// </summary>
        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                _isExecuting = value;
                RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region 构造

        public AsyncRelayCommand(Func<T, Task> execute, Func<T, bool> canExecute = null)
        {
            // ICommand.Execute(object)
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));            
            _canExecute = canExecute ?? new Func<T, bool>(a => true);
        }

        #endregion

        public override bool CanExecute(object parameter)
        {
            return !_isExecuting && _canExecute((T)parameter);
        }


        public override async void Execute(object parameter)
        {
            var typedParameter = (T)parameter;
            if (!_isExecuting && _canExecute(typedParameter))
            {
                IsExecuting = true; 

                try
                {
                    await _execute(typedParameter);
                }
                finally
                {
                    IsExecuting = false;
                }
            }
        }
    }
}