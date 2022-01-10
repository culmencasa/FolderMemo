using Autofac;
using FolderMemo.ServiceInterfaces;
using FolderMemo.ViewModels;
using MVVMLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Utils;

namespace FolderMemo.ViewModels
{
    /// <summary>
    /// 规则参见： https://docs.microsoft.com/en-us/windows/win32/shell/how-to-customize-folders-with-desktop-ini
    /// ConfirmFileOp	Set this entry to 0 to avoid a "You Are Deleting a System Folder" warning when deleting or moving the folder.
    /// NoSharing       Not supported under Windows Vista or later.Set this entry to 1 to prevent the folder from being shared.
    /// IconFile        If you want to specify a custom icon for the folder, set this entry to the icon's file name. The .ico file name extension is preferred, but it is also possible to specify .bmp files, or .exe and .dll files that contain icons. If you use a relative path, the icon is available to people who view the folder over the network. You must also set the IconIndex entry.
    /// IconIndex       Set this entry to specify the index for a custom icon.If the file assigned to IconFile only contains a single icon, set IconIndex to 0.
    /// InfoTip         Set this entry to an informational text string. It is displayed as an infotip when the cursor hovers over the folder.If the user clicks the folder, the information text is displayed in the folder's information block, below the standard information.
    /// </summary>
    public class SingleCommentViewModel : ViewModelBase
    {
        #region Construct

        public SingleCommentViewModel()
        {
            SaveCommand = new RelayCommand(SaveCommandAction);
            OpenFolderCommand = new RelayCommand(OpenFolderCommandAction);
            SelectIconCommand = new RelayCommand(SelectIconCommandAction);
            SwitchPageCommand = new RelayCommand(SwitchPageCommandAction);
            ResetCommand = new RelayCommand(ResetCommandAction);

            //对引用的编码使用 Encoding.RegisterProvider 函数进行注册
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }


        #endregion

        static readonly string DesktopINI = "desktop.ini";
        static readonly string ShellClassSection = ".ShellClassInfo";

        #region Fields

        private string _folderFullPath;
        private string _folderRemarks;
        private string _iconFileFullPath;
        

        #endregion

        #region Properties

        public string FolderFullPath
        {
            get => _folderFullPath;
            set
            {
                this.Set(ref _folderFullPath, value);

                OnFolderPathChanged();
            }
        }

        public string FolderRemarks
        {
            get => _folderRemarks;
            set => this.Set(ref _folderRemarks, value);
        }
        public string IconFileFullPath
        {
            get => _iconFileFullPath;
            set
            {
                var changed = Set(ref _iconFileFullPath, value);

                if (changed)
                {
                    OnIconFileChanged();
                }
            }
        }

        public Uri ImageUri
        {
            get;
            set;
        }

        #endregion


        #region Command

        public ICommand SaveCommand { get; set; }
        public ICommand OpenFolderCommand { get; set; }
        public ICommand SelectIconCommand { get; set; }

        public ICommand SwitchPageCommand { get; set; }

        public ICommand ResetCommand { get; set; }

        #endregion

        #region Command Actions

        private void SaveCommandAction()
        {
            if (!IsValidPath(FolderFullPath, true))
            {
                Messenger.Publish(new MessageToUI(App.GetLocalizeString("FolderPathErrorText")));
                return;
            }

            // 2022-01-10 允许备注为空
            //if (string.IsNullOrEmpty(FolderRemarks))
            //{
            //    Messenger.Publish(new MessageToUI(App.GetLocalizeString("FolderMemoEmptyErrorText")));
            //    return;
            //}

            if (!string.IsNullOrEmpty(IconFileFullPath))
            {
                if (!IsIconValid())
                {
                    // 保持不变, 也不再提示
                }
            }


            var targetFile = Path.Combine(FolderFullPath, DesktopINI);
            if (File.Exists(targetFile))
            {
                File.SetAttributes(FolderFullPath, FileAttributes.Normal);
                File.SetAttributes(targetFile, FileAttributes.Normal | FileAttributes.Archive);
            }


            IniFile iniFile = new IniFile();
            if (App.CurrentLocalization == 0)
            {
                iniFile.CustomEncoding = Encoding.GetEncoding("GB2312");
            }
            else
            {
                iniFile.CustomEncoding = Encoding.Default;
            }
            if (File.Exists(targetFile))
            {
                iniFile.Load(targetFile);
            }

            var section = iniFile.Section(".ShellClassInfo");
            section.Set("InfoTip", FolderRemarks);
            section.Set("IconFile", IconFileFullPath);
            section.Set("IconIndex", "0");
            iniFile.Save(targetFile);

            File.SetAttributes(targetFile, FileAttributes.System | FileAttributes.Hidden);
            File.SetAttributes(FolderFullPath, FileAttributes.System);

            //刷新图标
            Messenger.Publish(new MessageToUI(Intents.IconChanged, new object[] { targetFile }));
            Messenger.Publish(new MessageToUI(App.GetLocalizeString("SaveCompleteText")));

        }

        private void OpenFolderCommandAction()
        {
            var dialog = ViewModelLocator.Instance.ChangeDirectoryDialogService;
            dialog.ShowDialog(null, (result) =>
            {
                if (result != null)
                {
                    FolderFullPath = result.ToString();
                }
            });
        }

        private void SelectIconCommandAction()
        {
            var dialog = ViewModelLocator.Instance.SelectFileDialogService;
            dialog.ShowDialog(null, (result) =>
            {
                IconFileFullPath = result.ToString();

                ImageUri = new Uri(IconFileFullPath);
                OnPropertyChanged(nameof(ImageUri));
            });
        }

        private void SwitchPageCommandAction()
        {
            Messenger.Publish(new MessageToUI(Intents.PageSwitch, new object[] { "BatchCommentPageKey" }));
        }


        private void ResetCommandAction()
        {
            this.IconFileFullPath = null;
            this.FolderFullPath = null;
            this.FolderRemarks = null;
        }



        #endregion


        #region Private Methods

        private void OnIconFileChanged()
        {
            try
            {
                ImageUri = new Uri(IconFileFullPath, UriKind.RelativeOrAbsolute);
                OnPropertyChanged(nameof(ImageUri));
            }
            catch(Exception ex)
            {
                Messenger.Publish(new MessageToUI(ex.Message));
            }
        }

        private void OnFolderPathChanged()
        {
            if (string.IsNullOrEmpty(FolderFullPath))
                return;

            string targetFile = Path.Combine(FolderFullPath, DesktopINI);
            if (File.Exists(targetFile))
            {
                IniFile iniFile = new IniFile
                {
                    CustomEncoding = Encoding.GetEncoding("GB2312") 
                };
                iniFile.Load(targetFile);

                var section = iniFile.Section(ShellClassSection);
                FolderRemarks = section.Get("InfoTip");
                IconFileFullPath = section.Get("IconFile");
            }
        }

        private bool IsValidPath(string path, bool exactPath = true)
        {
            bool isValid = true;

            try
            {
                string fullPath = Path.GetFullPath(path);

                if (exactPath)
                {
                    string root = Path.GetPathRoot(path);
                    isValid = string.IsNullOrEmpty(root.Trim(new char[] { '\\', '/' })) == false;
                }
                else
                {
                    isValid = Path.IsPathRooted(path);
                }

                DirectoryInfo di = new DirectoryInfo(path);
                isValid = isValid && di.Exists;

            }
            catch (Exception)
            {
                isValid = false;
            }

            return isValid;
        }

        private bool IsIconValid()
        {
            FileInfo fi = new FileInfo(Environment.ExpandEnvironmentVariables(IconFileFullPath));
            if (!fi.Exists)
            {
                return false;
            }

            if (fi.Extension != ".ico")
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
