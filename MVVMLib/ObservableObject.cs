using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVMLib
{
    /// <summary>
    /// 继承INotifyPropertyChanged接口的基类
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// 属性更改通知
        /// </summary>
        /// <param name="propertyName"></param>
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 给指定属性对应的字段设置新值，并通知该属性更改
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="field">字段</param>
        /// <param name="newValue">新值</param>
        /// <param name="propertyName">要通知变更的属性名</param>
        /// <returns>如果属性值发生改变，则返回true， 否则false</returns>
        protected bool Set<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
