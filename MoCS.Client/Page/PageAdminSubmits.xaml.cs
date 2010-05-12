using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MoCS.Service.DataContracts;
using MoCS.Client.Model;
using System.Windows.Forms;
using System.Xml;

namespace MoCS.Client.Page
{
    /// <summary>
    /// Interaction logic for PageAdminSubmits.xaml
    /// </summary>
    public partial class PageAdminSubmits : System.Windows.Controls.Page
    {

        public PageAdminSubmits()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);


            ShowSubmits();
        }

        private void ShowSubmits()
        {
            MoCSServiceProxy proxy = new MoCSServiceProxy();
            Submits submits = proxy.GetTournamentTeamSubmits();
            //this collection also returns the started but not really submitted results

            Submits realSubmits = new Submits();
            var s = from x in submits
                    where x.SubmitId != "0"
                    select x;

            foreach (var x in s)
            {
                realSubmits.Add(x);
            }

            this.datagrid1.DataContext = realSubmits;
        }

  
        private MoCS.Service.DataContracts.Submit GetDetailedSubmit(string submitId)
        {
            Submits submits = (Submits)this.datagrid1.DataContext;
            var x = from submit in submits
                    where submit.SubmitId == submitId
                    select submit;


            MoCSServiceProxy proxy = new MoCSServiceProxy();

            foreach (var y in x)
            {
                MoCS.Service.DataContracts.Submit submitDetails = proxy.GetSubmitDetails(submitId);

                return submitDetails;
            }
            return null;

        }

        private void ButtonDetails_Click(object sender, RoutedEventArgs e)
        {
            string submitId = (string)((System.Windows.Controls.Button)sender).CommandParameter;

            MoCS.Service.DataContracts.Submit submitDetails = GetDetailedSubmit(submitId);

            if (submitDetails != null)
            {
                if (submitDetails.Details != null && submitDetails.Details.Length>0)
                {
                    SubmitDetailsWindow w = new SubmitDetailsWindow(submitDetails.Details);
                    w.ShowDialog();
                }
             }
        }

        private void ButtonAbort_Click(object sender, RoutedEventArgs e)
        {
            string submitId = (string)((System.Windows.Controls.Button)sender).CommandParameter;
            
            MoCSServiceProxy proxy = new MoCSServiceProxy();
            proxy.DeleteSubmit(submitId);
            ShowSubmits();
        }

        private void ButtonFileContents_Click(object sender, RoutedEventArgs e)
        {
            string submitId = (string)((System.Windows.Controls.Button)sender).CommandParameter;

            MoCS.Service.DataContracts.Submit submitDetails = GetDetailedSubmit(submitId);

            if (submitDetails != null)
            {
                if (submitDetails.Payload != null)
                {
                    System.Text.ASCIIEncoding enc = new ASCIIEncoding();
                    string text = enc.GetString(submitDetails.Payload);

                    SubmitDetailsWindow w = new SubmitDetailsWindow(text, true);
                    w.ShowDialog();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MoCSServiceProxy proxy = new MoCSServiceProxy();
            string result = proxy.GetTournamentReport(TeamSession.GetInstance().CurrentTournamentId);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.ShowDialog();


            if (sfd.FileName != null && sfd.FileName.Length > 0)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);
                doc.Save(sfd.FileName);
            }

        }
    
    
    }
}
