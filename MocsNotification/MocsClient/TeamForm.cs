using System;
using System.Windows.Forms;

namespace MocsClient
{
    public partial class TeamForm : Form
    {
        bool _allowClose = false;
        public TeamForm(string teamId)
        {
            InitializeComponent();
            textBoxTeamId.Text = teamId;
        }

        public string TeamId
        {
            get { return textBoxTeamId.Text.Trim(); }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (!ValidateTeamId())
            {
                return;
            }

            _allowClose = true;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private bool ValidateTeamId()
        {
            if (string.IsNullOrEmpty(textBoxTeamId.Text.Trim()))
            {
                MessageBox.Show(this, "Team name cannot be empty", "Team Identification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxTeamId.Focus();
                return false;
            }
            return true;
        }

        private void TeamForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !_allowClose;
        }


    }
}
