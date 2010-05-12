using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MocsTestClient.MocsServiceReference;

namespace MocsTestClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBoxMessageType.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageType messageType = MessageType.Info;
            if (comboBoxMessageType.SelectedItem !=null)
            {
                messageType = (MessageType)Enum.Parse(typeof(MessageType), comboBoxMessageType.SelectedItem.ToString());
            }

            using (NotifyClient notifyClient = new NotifyClient())
            {
                notifyClient.NotifyAll(messageType, DateTime.Now,textBoxTeamId.Text, textBoxCategory.Text, textBoxText.Text);
            }
        }
    }
}
