using FolderMemo.ServiceInterfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace FolderMemo.Services
{
    public class SelectFileDialogService : IOutputDialogService
    {
        public void ShowDialog(object input, Action<object> outputCallback)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "图标格式|*.ico";

            if (dialog.ShowDialog() == true)
            {
                outputCallback?.Invoke(dialog.FileName);
            }
        }
    }
}
