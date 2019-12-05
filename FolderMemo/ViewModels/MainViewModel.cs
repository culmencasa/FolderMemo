using FolderMemo.ServiceInterfaces;
using MVVMLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using Utils.Misc;

namespace WpfApp1.ViewModels
{
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

        private void OnIconFileChanged()
        {
            
        }

        public ICommand SaveCommand { get; set; }
        public ICommand OpenFolderCommand { get; set; }
        public ICommand SelectIconCommand { get; set; }
        public IOutputDialogService ChangeDirDialogService { get; set; }

        private void SaveCommandAction()
        {
            var targetFile = Path.Combine(FolderFullPath, "desktop.ini");
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
            section.Set("IconFile", @"C:\Resources\Icons\Misc\Ghost.ico");
            section.Set("IconIndex", "0");
            iniFile.Save(targetFile);

            File.SetAttributes(targetFile, FileAttributes.System | FileAttributes.Hidden);
            File.SetAttributes(FolderFullPath, FileAttributes.System);
        }

        private void OpenFolderCommandAction()
        {
            ChangeDirDialogService.ShowDialog(null, (result) =>
            {
                if (result != null)
                {
                    FolderFullPath = result.ToString();
                }
            });
        }

        private void SelectIconCommandAction()
        {

        }

    }
}
