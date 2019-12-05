using System;
using System.Collections.Generic;
using System.Text;

namespace FolderMemo.ServiceInterfaces
{

    public interface IOutputDialogService
    {
        void ShowDialog(object input, Action<object> outputCallback);
    }
}
