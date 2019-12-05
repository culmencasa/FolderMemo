using System;
using System.Windows.Input;

namespace MVVMLib
{
    /// <summary>
    /// 继承ICommand接口的Command抽象基类
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        #region ICommand成员

        /// <summary>
        /// 应该在Command变为可用或不可用时触发该事件, 基本上没用
        /// </summary>
        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object parameter) => true;

        public abstract void Execute(object parameter);

        #endregion

        #region 公开的方法

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

    }
}