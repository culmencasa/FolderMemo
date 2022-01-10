using FolderMemo.ServiceInterfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Utils.Enhancement;

namespace FolderMemo.Services
{
    /// <summary>
    /// 更改文件夹对话框
    /// </summary>
    public class ChangeDirectoryDialogService : IOutputDialogService
    {
        public void ShowDialog(object argument, Action<object> dilogCloseCallback)
        {
            #region 需要引用System.Windows.Forms

            //var sourceFolderDialog = new FolderSelectDialog();
            //sourceFolderDialog.Title = "选择文件夹";
            //sourceFolderDialog.InitialDirectory = argument.ToDefaultString();

            //var dialog = sourceFolderDialog.Show();
            //if (dialog)
            //{
            //    dilogCloseCallback?.Invoke(sourceFolderDialog.FileName);
            //}
            //else
            //{
            //}

            #endregion
            
            #region Microsoft.Win32的文件选择对话框也能选择文件夹，但貌似只能选择没有子目录的目录
            //OpenFileDialog folderBrowser = new OpenFileDialog
            //{
            //    ValidateNames = false,
            //    CheckFileExists = false,
            //    CheckPathExists = true,
            //    FileName = "Folder Selection."
            //};
            //if (folderBrowser.ShowDialog() == true)
            //{
            //    string folderPath = Path.GetDirectoryName(folderBrowser.FileName);
            //    dilogCloseCallback?.Invoke(folderPath);
            //}
            #endregion
            

            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.Description = App.GetLocalizeString("ChangeDirectoryDialogDescription"); // "选择要添加注释的文件夹";
            dialog.UseDescriptionForTitle = true;
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
                MessageBox.Show(App.GetLocalizeString("ChangeDirectoryDialogDescription"));

            var owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(p => p.IsEnabled && p.IsVisible);
            if ((bool)dialog.ShowDialog(owner))
            {
                dilogCloseCallback(dialog.SelectedPath);
            }
            else
            { 
            
            }
        }
    }
}
