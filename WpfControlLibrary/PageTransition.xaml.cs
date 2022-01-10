using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfControlLibrary
{
    /// <summary>
    /// PageTransition.xaml 的交互逻辑
    /// </summary>
    public partial class PageTransition : UserControl
    {
        public PageTransition()
        {
            InitializeComponent();
        }

		Stack<FrameworkElement> pages = new Stack<FrameworkElement>();

		public void Open(FrameworkElement page)
		{
			contentPresenter.Content = page;
		}

		public void ShowPage(FrameworkElement newPage)
		{
			pages.Push(newPage);

			Task.Factory.StartNew(() => ShowNewPage());
		}


		void ShowNewPage()
		{
			Dispatcher.Invoke((Action)delegate
			{
				if (contentPresenter.Content != null)
				{
					FrameworkElement oldPage = contentPresenter.Content as FrameworkElement;

					if (oldPage != null)
					{
						oldPage.Loaded -= newPage_Loaded;

						UnloadPage(oldPage);
					}
				}
				else
				{
					ShowNextPage();
				}

			});
		}


		void newPage_Loaded(object sender, RoutedEventArgs e)
		{
			Storyboard showNewPage = (Resources["FadeIn"] as Storyboard).Clone();

			showNewPage.Begin(contentPresenter);

			//CurrentPage = sender as UserControl;
		}

		void UnloadPage(FrameworkElement page)
		{
			Storyboard hidePage =  (Resources["FadeOut"] as Storyboard).Clone();
			hidePage.Completed += hidePage_Completed;

			hidePage.Begin(contentPresenter);
		}

		void hidePage_Completed(object sender, EventArgs e)
		{
			contentPresenter.Content = null;

			ShowNextPage();
		}
		void ShowNextPage()
		{
			FrameworkElement newPage = pages.Pop();
			newPage.Loaded += newPage_Loaded;

			contentPresenter.Content = newPage;
		}

	}
}
