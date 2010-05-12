using System;
using System.Windows.Forms;
using System.Net;

namespace MocsClient
{
    public partial class ConfigurationForm : Form
    {
        

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

        public ConfigurationForm( bool useMulticast, string serverIPAddress, int serverPort, bool enableLogging, string logFileName)
        {
            InitializeComponent();            
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

        private void buttonOK_Click(object sender, EventArgs e)
        {            
            try
            {
                IPAddress.Parse(textBoxIPAddress.Text);             
            }
            catch
            {
                MessageBox.Show(this, "Invalid IP Address entered", "Mocs Notification Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxIPAddress.Focus();
                return;
            }

            int serverPort;
            if (!Int32.TryParse(textBoxPort.Text, out serverPort))
            {
                MessageBox.Show(this, "Invalid Port Number entered", "Mocs Notification Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxPort.Focus();
                return;
            }

            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
