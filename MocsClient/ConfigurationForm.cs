using System;
using System.Windows.Forms;

namespace MocsClient
{
    public partial class ConfigurationForm : Form
    {       
        public string TeamId
        {
            get { return textBoxTeamId.Text; }
        }

        public bool UseMulticast
        {
            get { return checkBoxUseMulticast.Checked; }
        }

        public string ServerIPAddress
        {
            get { return textBoxIPAddress.Text; }
        }

        public int ServerPort
        {
            get { return Int32.Parse(textBoxPort.Text); }
        }

        public bool EnableLogging
        {
            get { return checkBoxEnableLogging.Checked; }
        }

        public string LogFileName
        {
            get { return textBoxLogFileName.Text; }
        }

        public ConfigurationForm(string teamId, bool useMulticast, string serverIPAddress, int serverPort, bool enableLogging, string logFileName)
        {
            InitializeComponent();
            textBoxTeamId.Text = teamId;
            textBoxIPAddress.Text = serverIPAddress;
            textBoxPort.Text = serverPort.ToString();
            checkBoxUseMulticast.Checked = useMulticast;
            checkBoxEnableLogging.Checked = enableLogging;
            textBoxLogFileName.Text = logFileName;
        }
  
        private void checkBoxUseMulticast_CheckStateChanged(object sender, EventArgs e)
        {
            textBoxIPAddress.Enabled = !checkBoxUseMulticast.Checked;
            textBoxPort.Enabled = !checkBoxUseMulticast.Checked;
        }
    }
}
