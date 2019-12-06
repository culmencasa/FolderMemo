using FolderMemo.Services;
using FolderMemo.ViewModels;
using MVVMLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utils.Enhancement;
using WpfApp1.ViewModels;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SimpleMessenger.Default.Subscribe<string>(this, message =>
            {
                if (message == Notifications.IconChanged)
                {
                    //var s = new System.Drawing.IconConverter();
                    //s.ConvertTo()
                    //using (Icon sysicon = .Icon.ExtractAssociatedIcon(FilePath))
                    //{
                    //    icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    //              sysicon.Handle,
                    //              System.Windows.Int32Rect.Empty,
                    //              System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    //}
                }
            });

            SimpleMessenger.Default.Subscribe<MessageToUI>(this, message =>
            {
                MessageBox.Show(message.Text);
            });
        }

        private void ContentButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
