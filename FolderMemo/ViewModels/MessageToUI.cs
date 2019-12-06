using System;
using System.Collections.Generic;
using System.Text;

namespace FolderMemo.ViewModels
{
    public class MessageToUI
    {
        public MessageToUI(string text)
        {
            Text = text;
        }

        public string Text
        {
            get;
            set;
        }
    }
}
