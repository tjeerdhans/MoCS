using System;
using System.Windows.Forms;

namespace MocsClient
{
    public partial class ChatForm : Form
    {
        public delegate void SendMessageEventHandler(string teamId, string text);

        public event SendMessageEventHandler MessageToSend;
        public ChatForm()
        {
            InitializeComponent();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (MessageToSend != null)
            {
                MessageToSend(textBoxTeamId.Text, textBoxData.Text);
            }
            Close();
        }
    }
}
