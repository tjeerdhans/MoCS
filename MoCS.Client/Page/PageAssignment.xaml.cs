using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;
using MoCS.Client.Model;
using MoCS.Service.DataContracts;

namespace MoCS.Client.Page
{
	/// <summary>
    /// Interaction logic for PageAssignment.xaml
	/// </summary>
    public partial class PageAssignment : System.Windows.Controls.Page
	{
        private TournamentViewModel _model;

        public PageAssignment()
		{
			InitializeComponent();
		}

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _model = TournamentViewModel.GetInstance();
            _model.StartPollingTeamSubmits();
            this.DataContext = _model;

            int teamTournamentAssignmentId = TeamSession.GetInstance().CurrentTeamTournamentAssignmentId;

            //first, check to see if the team is already working on this assignment. 
            MoCSServiceProxy proxy = new MoCSServiceProxy();
            MoCS.Service.DataContracts.Assignment teamAssignment = proxy.GetTeamAssignment();

            AssignmentViewModel assignment = ViewModelFactory.CreateAssignment(teamAssignment);
            TeamSession.GetInstance().CurrentAssignment = assignment;

            MenuManager.GetInstance().ShowInAssignmentMenu(true);

            List<AssignmentViewModel> bindList = new List<AssignmentViewModel>();
            bindList.Add(assignment);

            this.LB.DataContext = bindList;

            RichTextBoxSyntaxHighlighter highlighter = new RichTextBoxSyntaxHighlighter();

            this.richTextBox2.TextChanged += new TextChangedEventHandler(highlighter.TextChangedEventHandler);
            this.richTextBox3.TextChanged += new TextChangedEventHandler(highlighter.TextChangedEventHandler);
            this.richTextBox4.TextChanged += new TextChangedEventHandler(highlighter.TextChangedEventHandler);

            Encoding enc = new UTF8Encoding();
            this.richTextBox1.AppendText(enc.GetString(assignment.Files["Case"].Contents));
            this.richTextBox2.AppendText(enc.GetString(assignment.Files["InterfaceFile"].Contents));
            this.richTextBox3.AppendText(enc.GetString(assignment.Files["ClassFile"].Contents));
            this.richTextBox4.AppendText(enc.GetString(assignment.Files["NunitTestFileClient"].Contents));

        }



        private void button1_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "C Sharp files|*.cs";
            dlg.Multiselect = false;
            dlg.Title = "Search file to submit";
            dlg.ValidateNames = true;

            dlg.ShowDialog();

            if (dlg.FileName.Length > 0)
            {
                FileStream fs = File.OpenRead(dlg.FileName);
                fs.Position = 0;

                try
                {
                    TeamSession ts = TeamSession.GetInstance();

                    byte[] bytes = ConvertStreamToByteArray(fs);
                    fs.Close();

                    MoCSServiceProxy proxy = new MoCSServiceProxy();
                    proxy.Upload(bytes, dlg.SafeFileName);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private byte[] ConvertStreamToByteArray(Stream stream)
        {
            byte[] respBuffer = new byte[stream.Length];
            try
            {
                int bytesRead = stream.Read(respBuffer, 0, respBuffer.Length);
            }
            finally
            {
                stream.Close();
            }

            return respBuffer;
        }



        private AssignmentViewModel GetAssignment(int assignmentId)
        {
            return TournamentViewModel.GetInstance().CurrentAssignment;
        }


        private void buttonDownload_Click(object sender, RoutedEventArgs e)
        {

            AssignmentViewModel current = TeamSession.GetInstance().CurrentAssignment;
            if (current != null)
            {
                byte[] zip = TeamSession.GetInstance().CurrentAssignment.ZipFile;

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = current.Name + ".zip";
                sfd.ValidateNames = true;
                sfd.ShowDialog();

                if (sfd.FileName.Length > 0)
                {
                    File.WriteAllBytes(sfd.FileName, zip);
                }

            }
        }

        private void filecontent_Click(object sender, RoutedEventArgs e)
        {
            string submitId = (string)((Button)sender).CommandParameter;

            MoCSServiceProxy proxy = new MoCSServiceProxy();

            Submit submit = proxy.GetSubmitDetails(submitId);

            if (submit.FileContents != null && submit.FileContents.Length > 0)
            {
                SubmitDetailsWindow w = new SubmitDetailsWindow(submit.FileContents, true);



                w.ShowDialog();
            }
        }

        private void details_Click(object sender, RoutedEventArgs e)
        {
            string submitId = (string)((Button)sender).CommandParameter;

            MoCSServiceProxy proxy = new MoCSServiceProxy();
            Submit submit = proxy.GetSubmitDetails(submitId);

            if (submit.Details.Length > 0)
            {
                SubmitDetailsWindow w = new SubmitDetailsWindow(submit.Details);
                w.ShowDialog();
            }
        }
    }


 
}