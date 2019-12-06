using System;

namespace MVVMLib
{
    public class ViewModelBase : ObservableObject
    {
        private static readonly Lazy<bool> IsInDesignModeLazy = new Lazy<bool>(GetIsInDesignMode);

        #region 构造

        public ViewModelBase(IMessenger messenger = null)
        {
            Messenger = messenger ?? SimpleMessenger.Default;
        }

        #endregion

        /// <summary>
        /// 消息发送对象
        /// </summary>
        protected IMessenger Messenger { get; }

        /// <summary>
        /// 是否设计模式
        /// </summary>
        public static bool IsInDesignMode => IsInDesignModeLazy.Value;

        private static bool GetIsInDesignMode()
        {
#if NETFRAMEWORK
            var descriptor = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(
                System.ComponentModel.DesignerProperties.IsInDesignModeProperty, typeof(System.Windows.FrameworkElement));

            return (bool)descriptor.Metadata.DefaultValue;
#else
            // UWP.
            var uwp = Type.GetType("Windows.ApplicationModel.DesignMode, Windows.Foundation.UniversalApiContract, ContentType=WindowsRuntime");
            if (uwp != null)
            {
                return (bool)uwp.GetProperty("DesignModeEnabled").GetValue(null);
            }

            // Xamarin.Forms.
            var xamarin = Type.GetType("Xamarin.Forms.DesignMode, Xamarin.Forms.Core");
            if (xamarin != null)
            {
                return (bool)uwp.GetProperty("IsDesignModeEnabled").GetValue(null);
            }

            return false;
#endif
        }
    }
}