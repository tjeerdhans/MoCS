using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MoCS.Client.Business;
using MoCS.Client.Model;
using System.ServiceModel;
using System.ServiceModel.Web;
using MoCS.Service.DataContracts;
using System.Net;

namespace MoCS.Client.Page
{
    /// <summary>
    /// Interaction logic for PageTeamManagement.xaml
    /// </summary>
    public partial class PageTeamManagement : System.Windows.Controls.Page
    {
        MoCSServiceProxy _proxy = new MoCSServiceProxy();

        public PageTeamManagement()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            UpdateTeamsList();
        }

        private void UpdateTeamsList()
        {

            grid.DataContext = _proxy.GetTeams();
        }

    


        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            string id = (string)((Button)sender).CommandParameter;
            Team team = new Team();
            team.TeamId = id;
            _proxy.DeleteTeam(team);
            UpdateTeamsList();
        }


        private void grid_RowEditEnding(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            _proxy.UpdateTeam((Team)e.Row.Item);
        }

        private void grid_AutoGeneratingColumn(object sender, Microsoft.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            //use this way of changing column readonly,
            //because we still can use autogeneratecolumns. 
            //especially for the dropdown columns for the enums
            if(e.PropertyName=="TeamId" || e.PropertyName=="Points" || e.PropertyName=="TeamStatus")
            {
                e.Column.IsReadOnly = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Team team = new Team();
            team.TeamId = "-1";
            team.TeamStatus = TeamStatus.Active;
            team.TeamType = TeamType.Normal;
            team.Points = 0;
            team.Password = "password";
            team.TeamName = "TEAM" + DateTime.Now.Ticks.ToString();
            team.TeamMembers = "member1";

            _proxy.InsertTeam(team);

            UpdateTeamsList();
        }


    }

}
