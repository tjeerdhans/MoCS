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
using Microsoft.Windows.Controls;
using MoCS.Client.Business.Entities;
using MoCS.Client.Business;
using MoCS.Client.Model;
using System.ServiceModel;
using MoCS.Service.DataContracts;
using System.ServiceModel.Web;
using System.Collections.ObjectModel;

namespace MoCS.Client.Page
{
    /// <summary>
    /// Interaction logic for PageTournamentAssignments.xaml
    /// </summary>
    public partial class PageTournamentAssignments : System.Windows.Controls.Page
    {

       
        public PageTournamentAssignments()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            MoCSServiceProxy proxy = new MoCSServiceProxy();
            ObservableCollection<AssignmentViewModel> assignments = proxy.GetTournamentAssignments();
            this.datagrid1.DataContext = assignments;
        }
                
        private void ToggleAssignmentActivation(int id)
        {
            MoCSServiceProxy proxy = new MoCSServiceProxy();

            AssignmentViewModel model = null;
            foreach (AssignmentViewModel afvm in (ObservableCollection<AssignmentViewModel>)this.datagrid1.DataContext)
            {
                if (afvm.TournamentAssignmentId == id)
                {
                    model = afvm;
                    break;
                }
            }
            
            MoCS.Service.DataContracts.Assignment a = new MoCS.Service.DataContracts.Assignment();
            a.TournamentAssignmentId = id.ToString();
            a.TournamentId = model.TournamentId.ToString();
            a.Points1 = model.Points1;
            a.Points2 = model.Points2;
            a.Points3 = model.Points3;
            a.AssignmentOrder = model.AssignmentOrder;
            a.AssignmentId = model.AssignmentId.ToString();
            a.Active = !model.Active;

            proxy.UpdateTournamentAssignment(TeamSession.GetInstance().CurrentTournamentId.ToString(), id.ToString(), a);

            

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            object parameter = ((Button)sender).CommandParameter;

            if (parameter == null)
            {
                return;
            }
            
            int id = Convert.ToInt32(parameter.ToString());

            try
            {
                ToggleAssignmentActivation(id);
                MoCSServiceProxy proxy = new MoCSServiceProxy();
                ObservableCollection<AssignmentViewModel> assignments = proxy.GetTournamentAssignments();
                this.datagrid1.DataContext = assignments;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }





        }
    }
}
