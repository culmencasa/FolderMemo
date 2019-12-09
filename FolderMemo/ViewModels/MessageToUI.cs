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
            Type = MessageTypes.Text;
        }

        public MessageToUI(string intentName, object[] arguments)
        {
            Type = MessageTypes.Intent;
            IntentName = intentName;
            IntentArguments = arguments ?? (new object[0]);
        }


        public string Text
        {
            get;
            set;
        }

        public MessageTypes Type
        {
            get;
            set;
        }

        public string IntentName
        {
            get;
            set;
        }

        public object[] IntentArguments
        {
            get;
            set;
        }
    }
}
