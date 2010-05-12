using System;
using System.Collections.Generic;
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
using MoCS.Client.Model;
using System.Collections.ObjectModel;
using MoCS.Service.DataContracts;

namespace MoCS.Client.Page
{
	/// <summary>
    /// Interaction logic for PageWaitForAssignment.xaml
	/// </summary>


	public partial class PageWaitForAssignment : System.Windows.Controls.Page
	{

        public static RoutedCommand StartAssignment = new RoutedCommand("StartAssignmentCommand", typeof(PageWaitForAssignment));
        public static RoutedCommand ShowAssignment = new RoutedCommand("ShowAssignmentCommand", typeof(PageWaitForAssignment));


        private TournamentViewModel tvm = null;

        public PageWaitForAssignment()
		{
			InitializeComponent();

            MenuManager.GetInstance().ShowInAssignmentMenu(false);
            TeamSession.GetInstance().CurrentTeamTournamentAssignmentId = 0;
            TeamSession.GetInstance().CurrentAssignment = null;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            tvm = TournamentViewModel.GetInstance();
            this.DataContext = tvm;

            ShowContinueControls();
        }

        private void ShowContinueControls()
        {
            if (!TeamSession.GetInstance().IsAdmin)
            {
                MenuManager.GetInstance().ShowInAssignmentMenu(tvm.CurrentAssignment != null);
            }
        }

        void OnStartAssignment(object sender, ExecutedRoutedEventArgs e)
        {
            int tournamentAssignmentId = (int) e.Parameter;

            MoCSServiceProxy proxy = new MoCSServiceProxy();

            TeamSession ts = TeamSession.GetInstance();

            //start the assignment
            Assignment a = proxy.StartTeamTournamentAssignment(int.Parse(ts.CurrentTeam.TeamId),
                                        tournamentAssignmentId);

            TeamSession.GetInstance().CurrentTeamTournamentAssignmentId = int.Parse(a.TeamTournamentAssignmentId);
            MenuManager.GetInstance().NavigateTo("Page/PageAssignment.xaml");
        }

        void OnShowAssignment(object sender, ExecutedRoutedEventArgs e)
        {
            int teamTournamentAssignmentId = (int)e.Parameter;
            TeamSession.GetInstance().CurrentTeamTournamentAssignmentId = teamTournamentAssignmentId;
            MenuManager.GetInstance().NavigateTo("Page/PageAssignment.xaml");
        }

 
	}
}