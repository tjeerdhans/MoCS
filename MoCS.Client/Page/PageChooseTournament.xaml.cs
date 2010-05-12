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
using MoCS.Service.DataContracts;
using MoCS.Client.Model;

namespace MoCS.Client.Page
{
    /// <summary>
    /// Interaction logic for PageChooseTournament.xaml
    /// </summary>
    public partial class PageChooseTournament : System.Windows.Controls.Page
    {
        public static RoutedCommand ChooseTournament = new RoutedCommand("ChooseTournamentCommand", typeof(PageChooseTournament));

        public PageChooseTournament()
        {
            InitializeComponent();

            MenuManager.GetInstance().ShowInAssignmentMenu(false);
            TeamSession.GetInstance().CurrentTeamTournamentAssignmentId = 0;
            TeamSession.GetInstance().CurrentAssignment = null;
         //   TeamSession.GetInstance().CurrentTournamentId = 0;
         //   TeamSession.GetInstance().CurrentTournament = null;

            MoCSServiceProxy proxy = new MoCSServiceProxy();
            List<Tournament> tournaments = proxy.GetTournaments();

            this.PanelTournaments.DataContext = tournaments;
            
        }


        public void OnChooseTournament(object sender, ExecutedRoutedEventArgs e)
        {

            int id = int.Parse(e.Parameter.ToString());

            TeamSession.GetInstance().CurrentTournamentId = id;

            Tournament t = (from c in (List<Tournament>)this.PanelTournaments.DataContext
                            where c.TournamentId == id.ToString()
                            select c).FirstOrDefault();

            TeamSession.GetInstance().CurrentTournament = t;

            MenuManager.GetInstance().ShowLoggedInMenu(!TeamSession.GetInstance().IsAdmin);
            MenuManager.GetInstance().ShowAdminMenu(TeamSession.GetInstance().IsAdmin);

            if (TeamSession.GetInstance().IsAdmin)
            {
                MenuManager.GetInstance().NavigateTo("Page/PageAllSubmits.xaml");
            }
            else
            {
                MenuManager.GetInstance().NavigateTo("Page/PageWaitForAssignment.xaml");
            }

        }
    }
}
