using FolderMemo.ViewModels;
using FolderMemo.Views;
using MVVMLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FolderMemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Window

        public MainWindow()
        {
            InitializeComponent();

            this.Title = "Folder Memo";

            Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializePages();
            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    SwitchPage("SingleCommentPageKey");
                });
            });


            SimpleMessenger.Default.Subscribe<MessageToUI>(this, message =>
            {
                switch (message.Type)
                {
                    case MessageTypes.Text:
                        {
                            MessageBox.Show(message.Text, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        break;
                    case MessageTypes.Intent:
                        {
                            if (message.IntentName == Intents.IconChanged)
                            {
                                // 刷新图标2
                                //RefreshIcon();

                                // 刷新图标1(没效果）
                                //SHChangeNotify(HChangeNotifyEventID.SHCNE_DELETE, HChangeNotifyFlags.SHCNF_PATHA, message.IntentArguments[0].ToString(), IntPtr.Zero);
                                //SHChangeNotify(HChangeNotifyEventID.SHCNE_ASSOCCHANGED, HChangeNotifyFlags.SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);

                                // 刷新资源管理器
                                RefreshExplorer();
                            }
                            else if (message.IntentName == Intents.PageSwitch)
                            {
                                if (message.IntentArguments != null && message.IntentArguments.Length > 0)
                                {
                                    string keyName = message.IntentArguments[0].ToString();
                                    SwitchPage(keyName);
                                }
                            }
                        }
                        break;
                }
            });


            App.UseLocalizationCN();
        }

        #endregion



        #region Refresh Icon 3

        /// <summary>
        /// 
        /// </summary>
        private void RefreshExplorer()
        {
            // based on http://stackoverflow.com/questions/2488727/refresh-windows-explorer-in-win7
            Guid CLSID_ShellApplication = new Guid("13709620-C279-11CE-A49E-444553540000");
            Type shellApplicationType = Type.GetTypeFromCLSID(CLSID_ShellApplication, true);

            object shellApplication = Activator.CreateInstance(shellApplicationType);
            object windows = shellApplicationType.InvokeMember("Windows", System.Reflection.BindingFlags.InvokeMethod, null, shellApplication, new object[] { });

            Type windowsType = windows.GetType();
            object count = windowsType.InvokeMember("Count", System.Reflection.BindingFlags.GetProperty, null, windows, null);
            for (int i = 0; i < (int)count; i++)
            {
                object item = windowsType.InvokeMember("Item", System.Reflection.BindingFlags.InvokeMethod, null, windows, new object[] { i });
                Type itemType = item.GetType();

                string itemName = (string)itemType.InvokeMember("Name", System.Reflection.BindingFlags.GetProperty, null, item, null);
                if (itemName == "Windows Explorer" || itemName == "文件资源管理器")
                {
                    // 有可能等待10秒左右才能看到效果
                    itemType.InvokeMember("Refresh", System.Reflection.BindingFlags.InvokeMethod, null, item, null);
                }
            }
        }

        #endregion

        #region PageSwitch

        Dictionary<string, Lazy<UserControl>> pages;

        private void InitializePages()
        {
            pages = new Dictionary<string, Lazy<UserControl>>();
            pages.Add("SingleCommentPageKey", new Lazy<UserControl>(()=> new SingleCommentPage()));
            pages.Add("BatchCommentPageKey", new Lazy<UserControl>(() => new BatchCommentPage()));
        }

        private void SwitchPage(string pageKeyName)
        {
            if (pages.ContainsKey(pageKeyName))
            {
                var nextPage = pages[pageKeyName].Value;

                if (pageWrapper.Content != null)
                {
                    UserControl oldPage = pageWrapper.Content as UserControl;
                    if (oldPage != null)
                    {
                        AnimateUnloading(nextPage);
                    }
                }
                else
                {
                    nextPage.Loaded -= PageLoad;
                    nextPage.Loaded += PageLoad;
                    pageWrapper.Content = nextPage;
                }

            }
        }

        private void PageLoad(object sender, RoutedEventArgs e)
        {
            var page = sender as UserControl;
            AnimateLoading(page);
        }


        private void AnimateLoading(FrameworkElement target)
        {
            Storyboard showNewPage = (Resources["FadeIn"] as Storyboard).Clone();
            showNewPage.Begin(pageWrapper);
        }

        private void AnimateUnloading(FrameworkElement target)
        {
            Storyboard fadeOutSb = (Resources["FadeOut"] as Storyboard).Clone();
            fadeOutSb.Completed += (s, e) =>
            {
                target.Loaded -= PageLoad;
                target.Loaded += PageLoad;
                pageWrapper.Content = target;
            };

            fadeOutSb.Begin(pageWrapper);
        }

        #endregion

        private void btnLanguage_Click(object sender, RoutedEventArgs e)
        {
            App.SwitchLanguage();
        }


    }
}
