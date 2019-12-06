using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfControlLibrary
{
    public class HintTextbox : TextBox
    { 
        public string HintText
        {
            get
            {
                return (string)GetValue(HintTextProperty);
            }
            set
            {
                SetValue(HintTextProperty, value);
            }
        }

        public static DependencyProperty HintTextProperty = DependencyProperty.Register(
            nameof(HintText), typeof(string), typeof(HintTextbox));
    }
}
