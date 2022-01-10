using MVVMLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Input;
using Utils;

namespace FolderMemo.ViewModels
{
    public class BatchCommentViewModel : ViewModelBase
    {
        static readonly string DesktopINI = "desktop.ini";
        static readonly string ShellClassSection = ".ShellClassInfo";

        public ObservableCollection<string> BatchCommentFolders
        { get; set; }

        public BatchCommentViewModel()
        {
            BatchCommentFolders = new ObservableCollection<string>();

            SwitchPageCommand = new RelayCommand(SwitchPageCommandAction);
            RemoveCommand = new RelayCommand(RemoveCommandAction);
            RemoveAllCommand = new RelayCommand(RemoveAllCommandAction);
            SaveCommand = new RelayCommand(SaveCommandAction);
            ShowPathDialogCommand = new RelayCommand(ShowPathDialogCommandAction);
        }


        private string _selectedItemText;
        public string SelectedItemText { 
            get
            {
                return _selectedItemText;
            }
            set
            { 
                Set(ref _selectedItemText, value); 
            }
        }

        private string _folderRemarks;
        public string FolderRemarks
        {
            get
            {
                return _folderRemarks;
            }

            set
            {
                Set(ref _folderRemarks, value);
            }
        }
        public ICommand SwitchPageCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand RemoveAllCommand { get; set; }

        public ICommand ShowPathDialogCommand { get; set; }
        public ICommand SaveCommand { get; set; } 

        private void SwitchPageCommandAction()
        {
            Messenger.Publish(new MessageToUI(Intents.PageSwitch, new object[] { "SingleCommentPageKey" }));
        }
        private void RemoveCommandAction()
        {
            if (!string.IsNullOrEmpty(SelectedItemText))
            {
                if (BatchCommentFolders.Contains(SelectedItemText))
                { 
                    BatchCommentFolders.Remove(SelectedItemText); 
                }
            }
        }

        private void RemoveAllCommandAction()
        {
            BatchCommentFolders.Clear();
        }

        private void ShowPathDialogCommandAction()
        {
            
        }

        private void SaveCommandAction()
        {
            // 2022-01-10 允许备注为空
            //if (string.IsNullOrEmpty(FolderRemarks))
            //{
            //    Messenger.Publish(new MessageToUI(App.GetLocalizeString("FolderMemoEmptyErrorText")));
            //    return;
            //}

            foreach (string folderPath in BatchCommentFolders)
            {
                if (!IsValidPath(folderPath, true))
                {
                    Messenger.Publish(new MessageToUI(App.GetLocalizeString("FolderPathErrorText")));
                    return;
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
                var desktopINIFile = Path.Combine(folderPath, DesktopINI);
                if (File.Exists(desktopINIFile))
                {
                    File.SetAttributes(folderPath, FileAttributes.Normal);
                    File.SetAttributes(desktopINIFile, FileAttributes.Normal | FileAttributes.Archive);
                }

                if (File.Exists(desktopINIFile))
                {
                    iniFile.Load(desktopINIFile);
                }
                else
                {
                    iniFile = new IniFile
                    {
                        CustomEncoding = Encoding.GetEncoding("GB2312")
                    };
                }

                var section = iniFile.Section(".ShellClassInfo");
                section.Set("InfoTip", FolderRemarks);
                iniFile.Save(desktopINIFile);

                File.SetAttributes(desktopINIFile, FileAttributes.System | FileAttributes.Hidden);
                File.SetAttributes(folderPath, FileAttributes.System);
            }

            //刷新图标
            Messenger.Publish(new MessageToUI(Intents.IconChanged, null));
            Messenger.Publish(new MessageToUI(App.GetLocalizeString("SaveCompleteText")));

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
    }
}
