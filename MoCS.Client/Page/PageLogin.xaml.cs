using System;
using System.Windows;
using MoCS.Client.Model;
using MoCS.Service.DataContracts;
using System.Collections.Generic;
using System.ServiceModel.Web;
using System.ServiceModel;

namespace MoCS.Client.Page
{
    /// <summary>
    /// Interaction logic for PageLogin.xaml
    /// </summary>

    public partial class PageLogin : System.Windows.Controls.Page
    {
        public PageLogin()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            MenuManager.GetInstance().ShowLoggedInMenu(false);
            MenuManager.GetInstance().ShowInAssignmentMenu(false);

            TeamSession session = TeamSession.GetInstance();
            this.textBox1.Text = session.TeamName;

        }


        private void Login()
        {
            MenuManager.GetInstance().ShowLoggedInMenu(false);

            MoCS.Client.RestService.IMoCSService service = TeamSession.GetInstance().RestService;

            string teamname = this.textBox1.Text;
            string password = this.passwordBox1.Password;

            if (teamname.Length == 0)
            {
                this.label3.Content = "Invalid TeamName";
                this.label3.Visibility = Visibility.Visible;
                this.textBox1.SelectAll();
                this.textBox1.Focus();
                return;
            }

            if (password.Length == 0)
            {
                this.label3.Content = "Password is empty";
                this.label3.Visibility = Visibility.Visible;
                this.passwordBox1.Focus();
                return;
            }

            MoCSServiceProxy proxy = new MoCSServiceProxy();
            Team result = proxy.Login(teamname, password);

            if (result == null)
            {
                this.label3.Content = "Login failed";
                this.label3.Visibility = Visibility.Visible;
                this.passwordBox1.Clear();
                this.textBox1.SelectAll();
                this.textBox1.Focus();
            }


            if (result != null)
            {
                MenuManager.GetInstance().Menu.Visibility = Visibility.Visible;

                TeamSession.GetInstance().TeamName = teamname;
                TeamSession.GetInstance().Password = password;
                TeamSession.GetInstance().IsLoggedIn = true;
                TeamSession.GetInstance().CurrentTeam = result;

                bool isAdmin = (result.TeamType == TeamType.Administrator);

                TeamSession.GetInstance().IsAdmin = isAdmin;

                MenuManager.GetInstance().NavigateTo("Page/PageChooseTournament.xaml");


 
            }
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }


    }
}