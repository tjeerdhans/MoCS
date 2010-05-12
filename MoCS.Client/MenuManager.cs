using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Navigation;

namespace MoCS.Client
{
    public class MenuManager
    {
        private static MenuManager _instance;
        private Menu _menu;
        private NavigationService _navigationService;
        public static MenuManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MenuManager();
            }
            return _instance;
        }

        private MenuManager()
        {

        }

        public Menu Menu
        {
            get { return _menu; }
            set { _menu = value; }
        }

        public NavigationService NavigationService
        {
            get { return _navigationService; }
            set { _navigationService = value; }
        }

        public void NavigateTo(string pageName)
        {
            _navigationService.Navigate(new Uri((string)pageName, UriKind.Relative));
        }

        public void ShowInAssignmentMenu(bool show)
        {
            MenuItem mi = (MenuItem)_menu.Items[4];     //assignment
            if (show)
            {
                mi.Visibility = Visibility.Visible;

            }
            else
            {
                mi.Visibility = Visibility.Hidden;
            }

        }

        public void ShowLoggedInMenu(bool show)
        {
            //0 = login
            //1 = tournaments
            //2 = assignments
            //3 = submitmatrix
            //4 = assignment
            //5 = admin

            MenuItem mi = (MenuItem)_menu.Items[1];     //tournaments
            MenuItem mi2 = (MenuItem)_menu.Items[2];    //wait for assignment
            MenuItem mi3 = (MenuItem)_menu.Items[3];    //submitmatrix
            MenuItem mi4 = (MenuItem)_menu.Items[4];    //assignment
            MenuItem mi5 = (MenuItem)_menu.Items[5];    //admin

            if (show)
            {
                mi.Visibility = Visibility.Visible;
                mi2.Visibility = Visibility.Visible;
                mi3.Visibility = Visibility.Visible;
                mi4.Visibility = Visibility.Visible;
            }
            else
            {
                mi.Visibility = Visibility.Hidden;
                mi2.Visibility = Visibility.Hidden;
                mi3.Visibility = Visibility.Hidden;
                mi4.Visibility = Visibility.Hidden;
                mi5.Visibility = Visibility.Hidden;
            }


        }

        public void ShowAdminMenu(bool show)
        {   
            MenuItem admin = (MenuItem)_menu.Items[5];//admin 
            if (show)
            {
                admin.Visibility = Visibility.Visible;
            }
            else
            {
                admin.Visibility = Visibility.Hidden;
            }
        }
    }
}
