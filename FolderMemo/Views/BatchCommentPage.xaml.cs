using FolderMemo.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfControlLibrary;

namespace FolderMemo
{
    /// <summary>
    /// SetCommentInBatch.xaml 的交互逻辑
    /// </summary>
    public partial class BatchCommentPage : UserControl
    {
        public BatchCommentPage()
        {
            InitializeComponent();
        }

        public Page LastPage { get; set; }

        #region Folder Path Area Drag&Drop

        private void txtFolderName_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Link;
            e.Handled = true;
        }

        private void txtFolderName_PreviewDrop(object sender, DragEventArgs e)
        {
            var vm = this.DataContext as BatchCommentViewModel;

            foreach (string item in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                DirectoryInfo di = new DirectoryInfo(item);
                if (di.Exists)
                {
                    if (!vm.BatchCommentFolders.Contains(item))
                        vm.BatchCommentFolders.Add(item);
                }
            }
        }

        #endregion

        #region Save Button

        private void btnSave_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ContentButton b = e.Source as ContentButton;
            b.BorderBackground = Brushes.Chocolate;
            b.Foreground = Brushes.White;
        }

        private void btnSave_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ContentButton b = e.Source as ContentButton;
            b.BorderBackground = Brushes.White;
            b.Foreground = Brushes.Black;
        }

        #endregion
    }
}
