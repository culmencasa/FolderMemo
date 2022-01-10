using FolderMemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfControlLibrary;

namespace FolderMemo.Views
{
    /// <summary>
    /// SingleCommentPage.xaml 的交互逻辑
    /// </summary>
    public partial class SingleCommentPage : UserControl // 继承Page,渐变动画是从黑色到白色, 而不是透明到白色. 所以改从UserControl继承
    {
        public SingleCommentPage()
        {
            InitializeComponent();
        }


        #region Refresh Icon 1

        [DllImport("shell32.dll")]
        private static extern void SHChangeNotify(HChangeNotifyEventID wEventId, HChangeNotifyFlags uFlags, IntPtr dwItem1, IntPtr dwItem2);


        [Flags]
        private enum HChangeNotifyFlags
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
        private enum HChangeNotifyEventID
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

        #region Refresh Icon 2

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
            //ie4uinit - show

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



        #region Icon Area Drag&Drop 

        private void ctrlIconPreview_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void ctrlIconPreview_PreviewDrop(object sender, DragEventArgs e)
        {
            var vm = this.DataContext as SingleCommentViewModel;

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

        #endregion

        #region Folder Path Area Drag&Drop

        private void txtFolderName_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Link;
            e.Handled = true;
        }

        private void txtFolderName_PreviewDrop(object sender, DragEventArgs e)
        {
            var vm = this.DataContext as SingleCommentViewModel;

            foreach (string item in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                DirectoryInfo di = new DirectoryInfo(item);
                if (di.Exists)
                {
                    vm.FolderFullPath = item;
                }
            }
        }

        #endregion

        #region Save Button

        private void btnSave_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ContentButton b = e.Source as ContentButton;
            b.BorderBackground = Brushes.Chocolate;
            b.Foreground = Brushes.White;
        }

        private void btnSave_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ContentButton b = e.Source as ContentButton;
            b.BorderBackground = Brushes.White;
            b.Foreground = Brushes.Black;
        }

        #endregion

        #region Switch Button


        private void btnSwitchToBatch_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new BatchCommentPage();
        }


        #endregion
    }
}
