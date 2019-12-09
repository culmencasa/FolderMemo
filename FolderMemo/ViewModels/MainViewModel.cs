using Autofac;
using FolderMemo.ServiceInterfaces;
using FolderMemo.ViewModels;
using MVVMLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using Utils.Misc;

namespace WpfApp1.ViewModels
{
    /// <summary>
    /// 规则参见： https://docs.microsoft.com/en-us/windows/win32/shell/how-to-customize-folders-with-desktop-ini
    /// ConfirmFileOp	Set this entry to 0 to avoid a "You Are Deleting a System Folder" warning when deleting or moving the folder.
    /// NoSharing       Not supported under Windows Vista or later.Set this entry to 1 to prevent the folder from being shared.
    /// IconFile        If you want to specify a custom icon for the folder, set this entry to the icon's file name. The .ico file name extension is preferred, but it is also possible to specify .bmp files, or .exe and .dll files that contain icons. If you use a relative path, the icon is available to people who view the folder over the network. You must also set the IconIndex entry.
    /// IconIndex       Set this entry to specify the index for a custom icon.If the file assigned to IconFile only contains a single icon, set IconIndex to 0.
    /// InfoTip         Set this entry to an informational text string. It is displayed as an infotip when the cursor hovers over the folder.If the user clicks the folder, the information text is displayed in the folder's information block, below the standard information.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        public MainViewModel()
        {
            SaveCommand = new RelayCommand(SaveCommandAction);
            OpenFolderCommand = new RelayCommand(OpenFolderCommandAction);
            SelectIconCommand = new RelayCommand(SelectIconCommandAction);

            //对引用的编码使用 Encoding.RegisterProvider 函数进行注册
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        private string _folderFullPath;
        private string _folderRemarks;
        private string _iconFileFullPath;

        public string FolderFullPath
        {
            get => _folderFullPath;
            set => this.Set(ref _folderFullPath, value);
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

        private void OnIconFileChanged()
        {
            
        }

        public ICommand SaveCommand { get; set; }
        public ICommand OpenFolderCommand { get; set; }
        public ICommand SelectIconCommand { get; set; }

        private void SaveCommandAction()
        {
            var targetFile = Path.Combine(FolderFullPath, "desktop.ini");
            if (File.Exists(targetFile))
            {
                File.SetAttributes(FolderFullPath, FileAttributes.Normal);
                File.SetAttributes(targetFile, FileAttributes.Normal | FileAttributes.Archive);
            }


            IniFile iniFile = new IniFile
            {
                CustomEncoding = Encoding.GetEncoding("GB2312")
            };
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

            Messenger.Publish(new MessageToUI(Intents.IconChanged, new object[] { targetFile }));
            Messenger.Publish(new MessageToUI("已保存"));


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

    }




}
