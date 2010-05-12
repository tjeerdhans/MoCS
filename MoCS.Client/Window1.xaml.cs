using System;
using System.Windows;
using System.Windows.Input;

namespace MoCS.Client
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            //star rating control
                //http://www.codestrider.com/blog/read/WPFStarRatingControl.aspx

            MenuManager.GetInstance().Menu = menu;
            MenuManager.GetInstance().NavigationService = frmContent.NavigationService;
            menu.Visibility = Visibility.Hidden;

        }


        private void GoToPageExecuteHandler(object sender, ExecutedRoutedEventArgs e)
        {
            frmContent.NavigationService.Navigate(new Uri((string)e.Parameter, UriKind.Relative));
        }

        private void GoToPageCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


    }


}
