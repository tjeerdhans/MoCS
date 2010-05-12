using System;
using System.Windows.Forms;

namespace MocsClient
{
    public partial class ChatForm : Form
    {
        public string TeamId
        {
            get { return textBoxTeamId.Text; }
        }

        public string TextToSend
        {
            get { return textBoxData.Text.Trim(); }
        }

        public ChatForm()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {

         if(string.IsNullOrEmpty(textBoxData.Text.Trim()))
         {
            MessageBox.Show(this, "Please enter a message to send.", "Mocs Messaging", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            textBoxData.Focus();
            return;
         }

            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
