using FolderMemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using Utils;

namespace FolderMemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Current.DispatcherUnhandledException += CurrentApp_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // 另外, 程序即使以管理员身份运行, 拖动文件功能将失效.
            // 两个办法: 1.关闭UAC 2.保证程序和文件所在进程都是以管理员身份运行
            // 第2个方法无效, 因为新版的Windows资源管理器无法以管理员身份运行.


            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                MessageBox.Show(GetLocalizeString("AdminModeHint"));
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            var locator = FindResource("Locator") as ViewModelLocator;
            locator?.Dispose();
        }


        #region Localization

        private static int currentLocalization = 0;
        /// <summary>
        /// 0 表示中文 1 表示英文
        /// </summary>
        public static int CurrentLocalization
        {
            get
            {
                return currentLocalization;
            }
            set
            {
                bool isAboutToChange = currentLocalization != value;
                currentLocalization = value;
                if (isAboutToChange)
                {
                    LanguageChanged?.Invoke();
                }
            }
        }

        public static object LockObject = new object();

        public static event Action LanguageChanged;

        public static string GetLocalizeString(string keyName)
        {
            return Conversion.ToDefaultString(Application.Current.FindResource(keyName));
        }


        /// <summary>
        /// 在中文两种语言中切换
        /// </summary>
        public static void SwitchLanguage()
        {
            lock (LockObject)
            {
                if (App.CurrentLocalization == 1)
                {
                    App.UseLocalizationCN();
                }
                else
                {
                    App.UseLocalizationEN();
                }
            }
        }

        /// <summary>
        /// 加载英文字典资源
        /// </summary>
        public static void UseLocalizationEN()
        {
            Task.Run(() =>
            {
                AppendLocalizationResource(1);

            }).ContinueWith((n) =>
            {
                CurrentLocalization = 1;
            });
        }

        /// <summary>
        /// 加载中文字典资源
        /// </summary>
        public static void UseLocalizationCN()
        {
            Task.Run(() =>
            {
                AppendLocalizationResource(0);

            }).ContinueWith((n) =>
            {
                CurrentLocalization = 0;
            });
        }

        private static void AppendLocalizationResource(int languageIndex)
        {

            string languageSourceEN = @"Resources\StringResource.xaml";
            string languageSourceCN = @"Resources\StringResource.zh-CN.xaml";

            var currentResourceDictionaries = Application.Current.Resources.MergedDictionaries.ToList();
            var enResourceDictionary = currentResourceDictionaries.FirstOrDefault(d => d.Source.OriginalString.Equals(languageSourceEN));
            var cnResourceDictionary = currentResourceDictionaries.FirstOrDefault(d => d.Source.OriginalString.Equals(languageSourceCN));
            if (enResourceDictionary != null)
            {
                // 如果已存在, 则删除后加到末尾以生效
                Application.Current.Resources.MergedDictionaries.Remove(enResourceDictionary);
            }
            if (cnResourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(cnResourceDictionary);
            }

            switch (languageIndex)
            {
                case 0: // 中文
                    {
                        var targetResourceDictionary = new ResourceDictionary() { Source = new Uri(languageSourceCN, UriKind.Relative) };
                        Application.Current.Resources.MergedDictionaries.Add(targetResourceDictionary);
                    }
                    break;
                case 1: // 英文
                    {
                        var targetResourceDictionary = new ResourceDictionary() { Source = new Uri(languageSourceEN, UriKind.Relative) };
                        Application.Current.Resources.MergedDictionaries.Add(targetResourceDictionary);
                    }
                    break;
                default:
                    break;

            }
        }

        #endregion


        void CurrentApp_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            if (e.Exception is System.UnauthorizedAccessException)
            {
                // 不支持更改C盘文件夹的备注, 需要以管理员身份运行                
                //RunAsAdmin();

                MessageBox.Show(GetLocalizeString("UnauthorizedAccessText"));
            }
            else
            {
                MessageBox.Show(e.Exception.Message);
            }
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show((e.ExceptionObject as Exception).Message);
        }


        private void RunAsAdmin()
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;

            startInfo.Verb = "runas";
            try
            {
                System.Diagnostics.Process.Start(startInfo);
            }
            catch
            {
                return;
            }

            Application.Current.Shutdown();


        }
    }
}
