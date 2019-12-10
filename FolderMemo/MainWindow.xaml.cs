using FolderMemo.Services;
using FolderMemo.ViewModels;
using MVVMLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utils.Enhancement;
using WpfApp1.ViewModels;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
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
                                RefreshIcon();

                                // 刷新图标1(没效果）
                                //SHChangeNotify(HChangeNotifyEventID.SHCNE_DELETE, HChangeNotifyFlags.SHCNF_PATHA, message.IntentArguments[0].ToString(), IntPtr.Zero);
                                //SHChangeNotify(HChangeNotifyEventID.SHCNE_ASSOCCHANGED, HChangeNotifyFlags.SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);

                            }
                        }
                        break;
                }
            });
        }


        private void ContentButton_Click(object sender, RoutedEventArgs e)
        {

        }


        #region 刷新图标1

        [DllImport("shell32.dll")]
        public static extern void SHChangeNotify(HChangeNotifyEventID wEventId, HChangeNotifyFlags uFlags, IntPtr dwItem1, IntPtr dwItem2);


        [Flags]
        public enum HChangeNotifyFlags
        {
            SHCNF_DWORD = 0x0003,
            SHCNF_IDLIST = 0x0000,
            SHCNF_PATHA = 0x0001,
            SHCNF_PATHW = 0x0005,
            SHCNF_PRINTERA = 0x0002,
            SHCNF_PRINTERW = 0x0006,
            SHCNF_FLUSH = 0x1000,
            SHCNF_FLUSHNOWAIT = 0x2000
        }

        [Flags]
        public enum HChangeNotifyEventID
        {
            SHCNE_ALLEVENTS = 0x7FFFFFFF,
            SHCNE_ASSOCCHANGED = 0x08000000,
            SHCNE_ATTRIBUTES = 0x00000800,
            SHCNE_CREATE = 0x00000002,
            SHCNE_DELETE = 0x00000004,
            SHCNE_DRIVEADD = 0x00000100,
            SHCNE_DRIVEADDGUI = 0x00010000,
            SHCNE_DRIVEREMOVED = 0x00000080,
            SHCNE_EXTENDED_EVENT = 0x04000000,
            SHCNE_FREESPACE = 0x00040000,
            SHCNE_MEDIAINSERTED = 0x00000020,
            SHCNE_MEDIAREMOVED = 0x00000040,
            SHCNE_MKDIR = 0x00000008,
            SHCNE_NETSHARE = 0x00000200,
            SHCNE_NETUNSHARE = 0x00000400,
            SHCNE_RENAMEFOLDER = 0x00020000,
            SHCNE_RENAMEITEM = 0x00000001,
            SHCNE_RMDIR = 0x00000010,
            SHCNE_SERVERDISCONNECT = 0x00004000,
            SHCNE_UPDATEDIR = 0x00001000,
            SHCNE_UPDATEIMAGE = 0x00008000,
        }


        #endregion

        #region 刷新图标2

        private string getVersion()
        {
            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            //Variable to hold our return value
            string operatingSystem = "";

            if (os.Platform == PlatformID.Win32Windows)
            {
                //This is a pre-NT version of Windows
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = "95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                            operatingSystem = "98SE";
                        else
                            operatingSystem = "98";
                        break;
                    case 90:
                        operatingSystem = "Me";
                        break;
                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "2000";
                        else
                            operatingSystem = "XP";
                        break;
                    case 6:
                        if (vs.Minor == 0)
                            operatingSystem = "Vista";
                        else if (vs.Minor == 1)
                            operatingSystem = "7";
                        else if (vs.Minor == 2)
                            operatingSystem = "8";
                        else
                            operatingSystem = "8.1";
                        break;
                    case 10:
                        operatingSystem = "10";
                        break;
                    default:
                        break;
                }
            }
            //Make sure we actually got something in our OS check
            //We don't want to just return " Service Pack 2" or " 32-bit"
            //That information is useless without the OS version.
            if (operatingSystem != "")
            {
                //Got something.  Let's prepend "Windows" and get more info.
                operatingSystem = "Windows " + operatingSystem;
                //See if there's a service pack installed.
                if (os.ServicePack != "")
                {
                    //Append it to the OS name.  i.e. "Windows XP Service Pack 3"
                    operatingSystem += " " + os.ServicePack;
                }
                //Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
                //operatingSystem += " " + getOSArchitecture().ToString() + "-bit";
            }
            //Return the information we've gathered.
            return operatingSystem;

        }


        private void RefreshIcon()
        {
            string osVersion = getVersion();
            if (osVersion == "Windows 10" || osVersion == "Windows 8")
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "ie4uinit.exe";
                psi.Arguments = "-show";
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                Process.Start(psi);
            }
            else
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "ie4uinit.exe";
                psi.Arguments = "-ClearIconCache";
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                Process.Start(psi);
            }

        }


        #endregion


        private void ctrlIconPreview_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void ctrlIconPreview_PreviewDrop(object sender, DragEventArgs e)
        {
            var vm = this.DataContext as MainViewModel;

            foreach (string item in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                FileInfo fi = new FileInfo(item);
                if (fi.Extension == ".ico")
                {
                    vm.IconFileFullPath = item;
                }
                else
                {
                    MessageBox.Show("只支持 ICO 格式的图标。", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                return;
            }

        }

        private void txtFolderName_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Link;
            e.Handled = true;
        }

        private void txtFolderName_PreviewDrop(object sender, DragEventArgs e)
        {
            var vm = this.DataContext as MainViewModel;

            foreach (string item in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                DirectoryInfo di = new DirectoryInfo(item);
                if (di.Exists)
                {
                    vm.FolderFullPath = item;
                }
            }
        }
    }
}
