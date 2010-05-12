namespace MocsClient
{
    partial class MocsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewNotification = new System.Windows.Forms.DataGridView();
            this.ColumnMessageType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNotification)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewNotification
            // 
            this.dataGridViewNotification.AllowUserToAddRows = false;
            this.dataGridViewNotification.AllowUserToDeleteRows = false;
            this.dataGridViewNotification.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewNotification.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewNotification.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewNotification.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnMessageType,
            this.ColumnTime,
            this.ColumnCategory,
            this.ColumnText});
            this.dataGridViewNotification.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewNotification.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewNotification.Name = "dataGridViewNotification";
            this.dataGridViewNotification.ReadOnly = true;
            this.dataGridViewNotification.RowHeadersVisible = false;
            this.dataGridViewNotification.Size = new System.Drawing.Size(526, 253);
            this.dataGridViewNotification.TabIndex = 0;
            this.dataGridViewNotification.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridViewNotification_MouseClick);
            // 
            // ColumnMessageType
            // 
            this.ColumnMessageType.HeaderText = "Type";
            this.ColumnMessageType.Name = "ColumnMessageType";
            this.ColumnMessageType.ReadOnly = true;
            this.ColumnMessageType.Width = 80;
            // 
            // ColumnTime
            // 
            this.ColumnTime.HeaderText = "Time";
            this.ColumnTime.Name = "ColumnTime";
            this.ColumnTime.ReadOnly = true;
            this.ColumnTime.Width = 60;
            // 
            // ColumnCategory
            // 
            this.ColumnCategory.HeaderText = "Category";
            this.ColumnCategory.Name = "ColumnCategory";
            this.ColumnCategory.ReadOnly = true;
            this.ColumnCategory.Width = 90;
            // 
            // ColumnText
            // 
            this.ColumnText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnText.DefaultCellStyle = dataGridViewCellStyle1;
            this.ColumnText.HeaderText = "Text";
            this.ColumnText.Name = "ColumnText";
            this.ColumnText.ReadOnly = true;
            // 
            // MocsForm
            // 
            this.ClientSize = new System.Drawing.Size(526, 253);
            this.Controls.Add(this.dataGridViewNotification);
            this.Name = "MocsForm";
            this.Text = "Mocs Notification";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MocsForm_FormClosing);
            this.Load += new System.EventHandler(this.MocsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNotification)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewNotification;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnMessageType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnText;




    }
}

