using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfControlLibrary
{
    public class ContentButton : Button
    {
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        public Brush BorderBackground
        {
            get
            {
                return (Brush)GetValue(BorderBackgroundProperty);
            }
            set
            {
                SetValue(BorderBackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(ContentButton));

        public static readonly DependencyProperty BorderBackgroundProperty =
            DependencyProperty.Register(nameof(BorderBackground), typeof(Brush), typeof(ContentButton));

        
    }
}
