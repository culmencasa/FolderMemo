using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMLib
{
    /// <summary>
    /// 异步无参Command
    /// </summary>
    public class AsyncRelayCommand : CommandBase
    {
        #region 字段

        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private bool _isExecuting;

        #endregion

        #region 属性

        /// <summary>
        /// Command委托是否已执行
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

        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? new Func<bool>(() => true);
        }

        #endregion

        public override bool CanExecute(object parameter)
        {
            return !_isExecuting && _canExecute();
        }

        public override async void Execute(object parameter)
        {
            if (!_isExecuting && _canExecute())
            {
                IsExecuting = true;

                try
                {
                    await _execute();
                }
                finally
                {
                    IsExecuting = false;
                }
            }
        }
    }
}